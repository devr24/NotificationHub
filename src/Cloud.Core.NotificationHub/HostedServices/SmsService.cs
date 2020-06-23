using Cloud.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Cloud.Core.NotificationHub.HostedServices
{
    /// <summary>
    /// Class SmsService.
    /// Implements the <see cref="IHostedService" />
    /// </summary>
    /// <seealso cref="IHostedService" />
    public class SmsService : IHostedService
    {
        internal readonly ITelemetryLogger _telemetryLogger;
        internal readonly ILogger<SmsService> _logger;
        private readonly IReactiveMessenger _messenger;
        private int _messagesProcessed = 0;

        /// <summary>Initializes a new instance of the <see cref="SmsService"/> class.</summary>
        /// <param name="messengers">The messengers.</param>
        /// <param name="telemetryLogger">The telemetry logger.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="monitor">The monitor.</param>
        public SmsService(NamedInstanceFactory<IReactiveMessenger> messengers, ITelemetryLogger telemetryLogger, ILogger<SmsService> logger, MonitorService monitor)
        {
            _logger = logger;
            _telemetryLogger = telemetryLogger;
            _messenger = messengers["sms"];

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
            _messenger.StartReceive<object>().Subscribe(async message => {

                

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
