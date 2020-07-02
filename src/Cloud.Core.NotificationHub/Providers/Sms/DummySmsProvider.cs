using System.Threading.Tasks;
using Cloud.Core.NotificationHub.Models;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub.Providers.Sms
{
    public class DummySmsProvider : ISmsProvider
    {
        private readonly ILogger<DummySmsProvider> _logger;

        public DummySmsProvider(ILogger<DummySmsProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>Gets or sets the name for the implementor of the INamedInstance interface.</summary>
        /// <value>The name of this instance.</value>
        public string Name { get; set; }

        /// <summary>Sends the specified email.</summary>
        /// <param name="sms">The email to send.</param>
        public void Send(SmsMessage sms)
        {
            _logger.LogInformation($"Sms sent successfully, TO: {string.Join(",", sms.To)}, CONTENT: {sms.FullContent}");
        }

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="sms">The email to send.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SendAsync(SmsMessage sms)
        {
            _logger.LogInformation($"Sms async sent successfully, TO: {string.Join(",", sms.To)}, CONTENT: {sms.FullContent}");

            return Task.FromResult(sms);
        }
    }
}
