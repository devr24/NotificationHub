namespace Cloud.Core.NotificationHub.HostedServices
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Notification.Events;
    using Services;

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
                            ILogger<EmailService> logger, MonitorService monitor, IBlobStorage blobStorage, AppSettings settings)
        {
            _emailProviders = emailProviders;
            _logger = logger;
            _telemetryLogger = telemetryLogger;
            _messenger = messengers["email"];
            _blobStorage = blobStorage;
            _settings = settings;

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
    }
}
