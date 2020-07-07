using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cloud.Core.Notification.Events;
using Cloud.Core.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub.HostedServices
{
    /// <summary>
    /// Class SmsService.
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
        private int _bitlyGenerated = 0;
        private int _sasUrlsGenerated = 0;

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
            _telemetryLogger.LogMetric("Bitly Urls Created", _bitlyGenerated);

            Interlocked.Exchange(ref _messagesProcessed, 0);
            Interlocked.Exchange(ref _sasUrlsGenerated, 0);
            Interlocked.Exchange(ref _bitlyGenerated, 0);
        }

        /// <summary>start as an asynchronous operation.</summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start reading messages from Service Bus.
            _messenger.StartReceive<SmsEvent>().Subscribe(async message => 
            {
                if (!_smsProviders.GetInstanceNames().Contains(message.SmsProvider))
                {
                    _logger.LogWarning($"{message.SmsProvider} has no sms implementation, erroring message");
                    await _messenger.Error(message, $"{message.SmsProvider} has no implementation");
                }

                var smsProvider = _smsProviders[message.SmsProvider.ToString()];

                SmsMessage sms = message;

                if (message.AttachmentIds != null)
                {
                    foreach (var attachmentId in message.AttachmentIds)
                    {
                        var sourcePath = $"{_settings.AttachmentContainerName}/{attachmentId}";
                        var blobData = await _blobStorage.GetBlob(sourcePath, true);
                        var fileName = blobData.Metadata["name"];
                        var publicPath = $"{_settings.AttachmentContainerName}/public/{attachmentId}/{fileName}";

                        await ((Storage.AzureBlobStorage.BlobStorage)_blobStorage).CopyFile(sourcePath, publicPath);

                        // Get the long sas url for the public facing blob.
                        var sasUrl = await _blobStorage.GetSignedBlobAccessUrl(publicPath, new SignedAccessConfig(new List<AccessPermission> { AccessPermission.Read }));

                        // Get the short link for the sas url.  This is easier to use on the phone!
                        var shortLink = await _urlShortener.ShortenLink(new Uri(sasUrl));

                        sms.Links.Add(new SmsLink
                        {
                            Title = fileName,
                            Link = shortLink.ShortLink
                        });
                        Interlocked.Increment(ref _sasUrlsGenerated);
                        Interlocked.Increment(ref _bitlyGenerated);
                    }
                }
                var result = await smsProvider.SendAsync(sms);

                // When finished, complete the message.
                await _messenger.Complete(message);

                _logger.LogDebug($"Sms {(result ? "sent successfully": "failed to send")} to {sms.To.Count} recipients, Text: {sms.FullContent}{(sms.Links.Any() ? string.Join(",", sms.Links.Select(l => l.Link)) : "")}");

                Interlocked.Increment(ref _messagesProcessed);
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
    }
}
