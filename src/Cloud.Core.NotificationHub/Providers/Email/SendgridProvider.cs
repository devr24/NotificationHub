namespace Cloud.Core.NotificationHub.Providers.Email
{
    using System.Threading.Tasks;

    /// <summary>
    /// Class Sendgrid email provider.
    /// Implements the <see cref="IEmailProvider" />
    /// </summary>
    /// <seealso cref="IEmailProvider" />
    public class SendgridProvider : IEmailProvider
    {
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

        /// <summary>Sends the email asynchronously.</summary>
        /// <param name="email">The email to send.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> SendAsync(EmailMessage email)
        {
            throw new System.NotImplementedException();
        }
    }
}
