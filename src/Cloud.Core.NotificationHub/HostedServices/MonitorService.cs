using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace Cloud.Core.NotificationHub.HostedServices
{
    /// <summary>Monitor Service class tracks a constant tick throughout the application.</summary>
    public class MonitorService
    {
        private readonly ILogger<MonitorService> _logger;
        private readonly Stopwatch _elapsedTime;

        /// <summary>Gets the name of the application.</summary>
        /// <value>The name of the application.</value>
        public string AppName => AppDomain.CurrentDomain.FriendlyName;

        /// <summary>
        /// Gets or sets the background timer tick action event - used to hook into the background timer tick to allow custom logs to be written.
        /// </summary>
        /// <value>The background timer tick.</value>
        public Action<TimeSpan> BackgroundTimerTick { get; set; } = null;

        /// <summary>Initializes a new instance of the <see cref="MonitorService"/> class.</summary>
        /// <param name="settings">Application settings.</param>
        /// <param name="logger">The logger.</param>
        public MonitorService(AppSettings settings, ILogger<MonitorService> logger)
        {
            _logger = logger;
            _elapsedTime = new Stopwatch();
            _elapsedTime.Start();
            var monitor = new Timer(_ =>
            {
                var timespan = _elapsedTime.Elapsed;
                _logger.LogDebug($"{AppDomain.CurrentDomain.FriendlyName} running time: {timespan:dd} day(s) {timespan:hh}:{timespan:mm}:{timespan:ss}.{timespan:fff}");
                BackgroundTimerTick?.Invoke(timespan);
            }, null, TimeSpan.FromSeconds(settings.MonitorFrequencySeconds), TimeSpan.FromSeconds(settings.MonitorFrequencySeconds));
        }
    }
}
