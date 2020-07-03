using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub.Providers.Email
{
    public class DummyEmailProvider : IEmailProvider
    {
        private readonly ILogger<DummyEmailProvider> _logger;

        public DummyEmailProvider(ILogger<DummyEmailProvider> logger)
        {
            _logger = logger;
        }

        /// <summary>Gets or sets the name for the implementor of the INamedInstance interface.</summary>
        /// <value>The name of this instance.</value>
        public string Name { get; set; }

        /// <summary>Sends the specified email.</summary>
        /// <param name="email">The email to send.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Send(EmailMessage email)
        {
            SendAsync(email).GetAwaiter().GetResult();
        }

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="email">The email to send.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SendAsync(EmailMessage email)
        {
            _logger.LogInformation($"Email sent successfully, TO: {string.Join(",", email.To)}, CONTENT: {email.FullContent}, TEMPLATE: {email.TemplateName}");

            return Task.FromResult(email);
        }
    }
}
