namespace Cloud.Core.NotificationHub.HostedServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Notification;
    using Notification.Events;
    using Services;
    using Template.HtmlMapper;

    /// <summary>
    /// Class Email Service will process email related notification messages.
    /// Implements the <see cref="IHostedService" />
    /// </summary>
    /// <seealso cref="IHostedService" />
    public class EmailService : IHostedService
    {
        private readonly ITelemetryLogger _telemetryLogger;
        private readonly ILogger<EmailService> _logger;
        private readonly IReactiveMessenger _messenger;
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings _settings;
        private readonly NamedInstanceFactory<IEmailProvider> _emailProviders;
        private readonly ITemplateMapper _templateMapper;
        private int _messagesProcessed = 0;
        private int _messageAttachments = 0;
        private int _messagesFailed = 0;
        private const string DefaultContentType = "application/octet-stream";

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailService" /> class.
        /// </summary>
        /// <param name="emailProviders">The email providers.</param>
        /// <param name="messengers">The messengers.</param>
        /// <param name="telemetryLogger">The telemetry logger.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="monitor">The monitor.</param>
        /// <param name="blobStorage">The BLOB storage.</param>
        /// <param name="settings">The settings.</param>
        public EmailService(NamedInstanceFactory<IEmailProvider> emailProviders, NamedInstanceFactory<IReactiveMessenger> messengers, ITelemetryLogger telemetryLogger, 
                            ILogger<EmailService> logger, MonitorService monitor, IBlobStorage blobStorage, AppSettings settings, ITemplateMapper templateMapper)
        {
            _emailProviders = emailProviders;
            _logger = logger;
            _telemetryLogger = telemetryLogger;
            _messenger = messengers["email"];
            _blobStorage = blobStorage;
            _settings = settings;
            _templateMapper = templateMapper;

            // Hook into the background timer.
            monitor.BackgroundTimerTick += LogBackgroundMetric;
        }

        /// <summary>Logs the background metric.</summary>
        /// <param name="elapsed">The elapsed.</param>
        public void LogBackgroundMetric(TimeSpan elapsed)
        {
            _logger.LogInformation($"Processed {_messagesProcessed} messages and {_messageAttachments} attachments");

            // Customise your statistics logging here.
            _telemetryLogger.LogMetric("Messages Processed", _messagesProcessed);
            _telemetryLogger.LogMetric("Messages Failed", _messagesFailed);
            _telemetryLogger.LogMetric("Message Attachments", _messageAttachments);

            Interlocked.Exchange(ref _messagesProcessed, 0);
            Interlocked.Exchange(ref _messagesFailed, 0);
            Interlocked.Exchange(ref _messageAttachments, 0);
        }

        /// <summary>start as an asynchronous operation.</summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start reading messages from Service Bus.
            _messenger.StartReceive<EmailEvent>(1).Subscribe(async message => 
            {
                try
                {
                    // Default the provider if not set.
                    if (message.Provider.IsNullOrEmpty())
                        message.Provider = _settings.DefaultSmsProvider.ToString();

                    if (!_emailProviders.TryGetValue(message.Provider, out var emailProvider))
                    {
                        throw new InvalidOperationException($"No implementation for provider {message.Provider}");
                    }

                    EmailMessage email = message;
                    email.Content = await GetEmailContent(message);

                    // Add attachment links (if attachments are requested).
                    if (message.AttachmentIds != null)
                    {
                        // Create a public link for the attachment.
                        foreach (var attachmentId in message.AttachmentIds)
                            email.Attachments.Add(await GetAttachment(attachmentId));
                    }

                    // Send the mail.
                    await emailProvider.SendAsync(email);

                    // When finished, complete the message.
                    await _messenger.Complete(message);
                    _logger.LogDebug($"Email sent successfully. To {email.To.Count} recipients, subject: {email.Subject}");
                    Interlocked.Increment(ref _messagesProcessed);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await _messenger.Error(message, ex.Message);
                    Interlocked.Increment(ref _messagesFailed);
                }
            });

            return Task.FromResult(true);
        }

        /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns>Task.</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(true);
        }

        /// <summary>Gets the attachment from storage using the attachment Id.</summary>
        /// <param name="attachmentId">The attachment identifier to lookup.</param>
        /// <returns>EmailAttachment.</returns>
        /// <exception cref="System.InvalidOperationException">Cannot find attachment at {path}</exception>
        private async Task<EmailAttachment> GetAttachment(Guid attachmentId)
        {
            var path = $"{_settings.AttachmentContainerName}/{attachmentId}";

            // Look the attachment up in storage.
            var blobData = await _blobStorage.GetBlob(path, true);
            if (blobData == null)
                throw new InvalidOperationException($"Cannot find attachment at {path}");

            if (!blobData.Metadata.TryGetValue("type", out var contentType))
                contentType = DefaultContentType;

            Interlocked.Increment(ref _messageAttachments);

            return new EmailAttachment { 
                Content = await _blobStorage.DownloadBlob(path), 
                Name = blobData.Metadata["name"], 
                ContentType = contentType 
            };
        }

        private async Task<string> GetEmailContent(EmailEvent email)
        {
            if (email.TemplateId.IsNullOrEmpty())
                return await Task.FromResult(email.Content.ToString());


            var template = await _templateMapper.GetTemplateContent(email.TemplateId) as HtmlTemplateResult;

            if (template == null || template.TemplateFound == false)
            {
                return null;
            }

            // HACK
            var obj = JsonConvert.DeserializeObject<dynamic>(email.Content.ToString());
            JToken outer = JToken.Parse(obj.ToString());
            JObject inner = outer.Root.Value<JObject>();

            //List<string> keys = inner.Properties().Select(p => p.Name).ToList();

            //foreach (string k in keys)
            //{
            //    Console.WriteLine(k);
            //}
            var innerDic = (Dictionary<string, object>)ToCollections(inner);

            var content = SubstituteTemplateValues(template.TemplateKeys, innerDic, template.TemplateContent);
            return content;
        }


        private object ToCollections(object o, string prefix = "")
        {
            if (!prefix.IsNullOrEmpty())
                prefix += ".";
            if (o is JObject jo) return jo.ToObject<IDictionary<string, object>>().ToDictionary(k => $"{prefix}{k.Key}", v => ToCollections(v.Value, $"{prefix}{v.Key}"));
            if (o is JArray ja) return ja.ToObject<List<object>>().Select(s => ToCollections(s)).ToList();
            return o;
        }

        internal string SubstituteTemplateValues(List<string> templateKeys, Dictionary<string, object> modelKeyValues, string templateContent)
        {
            var keyValuesToReplace = new Dictionary<string, string>();

            // Replace each key in the template with the models information.
            foreach (var k in templateKeys)
            {
                keyValuesToReplace.Add($"{{{{{k}}}}}", modelKeyValues[k.ToLowerInvariant()].ToString());
            }

            return templateContent.ReplaceMultiple(keyValuesToReplace);
        }
    }
}
