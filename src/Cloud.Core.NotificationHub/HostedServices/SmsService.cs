namespace Cloud.Core.NotificationHub.HostedServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Cloud.Core.Notification;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Notification.Events;
    using Services;

    /// <summary>
    /// Class Sms Service will process sms related notification messages.
    /// Implements the <see cref="IHostedService" />
    /// </summary>
    /// <seealso cref="IHostedService" />
    public class SmsService : IHostedService
    {
        private readonly ITelemetryLogger _telemetryLogger;
        private readonly ILogger<SmsService> _logger;
        private readonly IReactiveMessenger _messenger;
        private readonly IBlobStorage _blobStorage;
        private readonly AppSettings _settings;
        private readonly NamedInstanceFactory<ISmsProvider> _smsProviders;
        private readonly IUrlShortener _urlShortener;
        private int _messagesProcessed = 0;
        private int _shortLinksGenerated = 0;
        private int _sasUrlsGenerated = 0;
        private int _messagesFailed = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsService" /> class.
        /// </summary>
        /// <param name="smsProviders">The SMS providers.</param>
        /// <param name="messengers">The messengers.</param>
        /// <param name="telemetryLogger">The telemetry logger.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="monitor">The monitor.</param>
        /// <param name="blobStorage">The BLOB storage.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="urlShortener">The URL shortener.</param>
        public SmsService(NamedInstanceFactory<ISmsProvider> smsProviders, NamedInstanceFactory<IReactiveMessenger> messengers, ITelemetryLogger telemetryLogger,
                            ILogger<SmsService> logger, MonitorService monitor, IBlobStorage blobStorage, AppSettings settings, IUrlShortener urlShortener)
        {
            _smsProviders = smsProviders;
            _logger = logger;
            _telemetryLogger = telemetryLogger;
            _messenger = messengers["sms"];
            _blobStorage = blobStorage;
            _settings = settings;
            _urlShortener = urlShortener;

            // Hook into the background timer.
            monitor.BackgroundTimerTick += LogBackgroundMetric;
        }

        /// <summary>Logs the background metric.</summary>
        /// <param name="elapsed">The elapsed.</param>
        public void LogBackgroundMetric(TimeSpan elapsed)
        {
            _logger.LogInformation($"Processed {_messagesProcessed} messages and {_sasUrlsGenerated} attachments");

            // Customise your statistics logging here.
            _telemetryLogger.LogMetric("Messages Processed", _messagesProcessed);
            _telemetryLogger.LogMetric("Sas Urls Created", _sasUrlsGenerated);
            _telemetryLogger.LogMetric("Bitly Urls Created", _shortLinksGenerated);
            _telemetryLogger.LogMetric("Messages Failed", _messagesFailed);

            Interlocked.Exchange(ref _messagesProcessed, 0);
            Interlocked.Exchange(ref _messagesFailed, 0);
            Interlocked.Exchange(ref _sasUrlsGenerated, 0);
            Interlocked.Exchange(ref _shortLinksGenerated, 0);
        }

        /// <summary>start as an asynchronous operation.</summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start reading messages from Service Bus.
            _messenger.StartReceive<SmsEvent>().Subscribe(async message =>
            {
                try
                {
                    // Default the provider if not set.
                    if (message.SmsProvider.IsNullOrEmpty())
                        message.SmsProvider = _settings.DefaultSmsProvider.ToString();

                    if (!_smsProviders.TryGetValue(message.SmsProvider, out var smsProvider))
                    {
                        throw new InvalidOperationException($"No implementation for provider {message.SmsProvider}");
                    }

                    SmsMessage sms = message;

                    // Add attachment links (if attachments are requested).
                    if (message.AttachmentIds != null)
                    {
                        // Create a public link for the attachment.
                        foreach (var attachmentId in message.AttachmentIds)
                            sms.Links.Add(await GetAttachmentLink(attachmentId));
                    }

                    // Send the Sms and complete the message.
                    await smsProvider.SendAsync(sms);

                    // When finished, complete the message.
                    await _messenger.Complete(message);
                    _logger.LogDebug($"Sms sent successfully. To {sms.To.Count} recipients, Text: {sms.FullContent}{(sms.Links.Any() ? string.Join(",", sms.Links.Select(l => l.Link)) : "")}");
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

        /// <summary>
        /// Gets the attachment link by moving the attchment to a public access folder, then creating a sasUrl which is (.
        /// </summary>
        /// <param name="attachmentId">The attachment identifier.</param>
        /// <returns>SmsLink.</returns>
        /// <exception cref="System.InvalidOperationException">Cannot find attachment at {path}</exception>
        /// <exception cref="System.InvalidOperationException">Could not generate shortened link for {path} [Public path: {publicPath}, Sas url: {sasUrl}]</exception>
        private async Task<SmsLink> GetAttachmentLink(Guid attachmentId)
        {
            var path = $"{_settings.AttachmentContainerName}/{attachmentId}";

            // Look the attachment up in storage.
            var blobData = await _blobStorage.GetBlob(path, true);
            if (blobData == null)
                throw new InvalidOperationException($"Cannot find attachment at {path}");

            var fileName = blobData.Metadata["name"];
            var publicPath = $"{_settings.AttachmentContainerName}/public/{attachmentId}/{fileName}";

            // Server side file copy to a public directory.
            await((Storage.AzureBlobStorage.BlobStorage)_blobStorage).CopyFile(path, publicPath);

            // Get the long sas url for the public facing blob.
            var sasUrl = await _blobStorage.GetSignedBlobAccessUrl(publicPath, new SignedAccessConfig(new List<AccessPermission> { AccessPermission.Read }));
            Interlocked.Increment(ref _sasUrlsGenerated);

            // Get the short link for the sas url.  This is easier to use on the phone!
            var shortLink = await _urlShortener.ShortenLink(new Uri(sasUrl));
            if (!shortLink.Success)
                throw new InvalidOperationException($"Could not generate shortened link for {path} [Public path: {publicPath}, Sas url: {sasUrl}]");

            Interlocked.Increment(ref _shortLinksGenerated);

            return new SmsLink{
                Title = fileName,
                Link = shortLink.ShortLink
            };
        }
    }
}
