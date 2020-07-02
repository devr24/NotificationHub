using System;
using System.Threading;
using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Models.Events;
using Cloud.Core.NotificationHub.Providers;
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
        private int _messagesProcessed = 0;

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
        public SmsService(NamedInstanceFactory<ISmsProvider> smsProviders, NamedInstanceFactory<IReactiveMessenger> messengers, ITelemetryLogger telemetryLogger,
                            ILogger<SmsService> logger, MonitorService monitor, IBlobStorage blobStorage, AppSettings settings)
        {
            _smsProviders = smsProviders;
            _logger = logger;
            _telemetryLogger = telemetryLogger;
            _messenger = messengers["sms"];
            _blobStorage = blobStorage;
            _settings = settings;

            // Hook into the background timer.
            monitor.BackgroundTimerTick += LogBackgroundMetric;
        }

        /// <summary>Logs the background metric.</summary>
        /// <param name="elapsed">The elapsed.</param>
        public void LogBackgroundMetric(TimeSpan elapsed)
        {
            _logger.LogInformation($"Processed {_messagesProcessed} messages");

            // Customise your statistics logging here.
            _telemetryLogger.LogMetric("Messages Processed", _messagesProcessed);

            Interlocked.Exchange(ref _messagesProcessed, 0);
        }

        /// <summary>start as an asynchronous operation.</summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Start reading messages from Service Bus.
            _messenger.StartReceive<SmsEvent>().Subscribe(async message => {

                var smsProvider = _smsProviders[message.Provider.ToString()];

                SmsMessage sms = message;

                foreach (var attachmentId in message.AttachmentIds)
                {
                    var path = $"{_settings.AttachmentContainerName}/{attachmentId}";
                    var blobData = await _blobStorage.GetBlob(path, true);

                    var sasUrl = await _blobStorage.GetSignedBlobAccessUrl(path, new SignedAccessConfig(new System.Collections.Generic.List<AccessPermission> { AccessPermission.Read }, null));

                    sms.Links.Add(new ResourceLink { 
                        Name = blobData.Metadata["name"],
                        Link = new Uri(sasUrl)
                    });
                }

                await smsProvider.SendAsync(sms);

                // When finished, complete the message.
                await _messenger.Complete(message);

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
