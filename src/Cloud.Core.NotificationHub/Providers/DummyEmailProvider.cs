using System.Threading.Tasks;
using Cloud.Core.Notification;
using Microsoft.Extensions.Logging;

namespace Cloud.Core.NotificationHub.Providers
{
    /// <summary>
    /// Dummy Email Provider.
    /// Implements the <see cref="IEmailProvider" />
    /// </summary>
    /// <seealso cref="IEmailProvider" />
    public class DummyEmailProvider : IEmailProvider
    {
        private readonly ILogger<DummyEmailProvider> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DummyEmailProvider"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
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
        public bool Send(EmailMessage email)
        {
            return SendAsync(email).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sends an email synchronously.
        /// </summary>
        /// <param name="templatedEmail">The templated email.</param>
        /// <returns><c>True</c> if sent successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Send(EmailTemplateMessage templatedEmail)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="email">The email to send.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> SendAsync(EmailMessage email)
        {
            _logger.LogInformation($"Email sent successfully, TO: {string.Join(",", email.To)}, CONTENT: {email.Content}");

            return Task.FromResult(true);
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="templatedEmail">The templated email to send.</param>
        /// <returns><c>True</c> if sent successfully, <c>false</c> otherwise.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> SendAsync(EmailTemplateMessage templatedEmail)
        {
            throw new System.NotImplementedException();
        }
    }
}
