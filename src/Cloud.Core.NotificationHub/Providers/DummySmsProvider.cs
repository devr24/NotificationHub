using System;
using System.Threading.Tasks;
using Cloud.Core.Notification;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub.Providers
{
    /// <summary>
    /// Dummy Sms Provider.
    /// Implements the <see cref="ISmsProvider" />
    /// </summary>
    /// <seealso cref="ISmsProvider" />
    public class DummySmsProvider : ISmsProvider
    {
        private readonly ILogger<DummySmsProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummySmsProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public DummySmsProvider(ILogger<DummySmsProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>Gets or sets the name for the implementor of the INamedInstance interface.</summary>
        /// <value>The name of this instance.</value>
        public string Name { get; set; }

        /// <summary>Sends the specified email.</summary>
        /// <param name="sms">The email to send.</param>
        public bool Send(SmsMessage sms)
        {
            return SendAsync(sms).GetAwaiter().GetResult();
        }

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="sms">The email to send.</param>
        /// <returns>Task.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> SendAsync(SmsMessage sms)
        {
            _logger.LogInformation($"Sms async sent successfully, TO: {string.Join(",", sms.To)}, CONTENT: {sms.FullContent}");

            return Task.FromResult(true);
        }
    }
}
