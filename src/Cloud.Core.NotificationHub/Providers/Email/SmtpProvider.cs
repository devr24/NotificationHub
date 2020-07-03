namespace Cloud.Core.NotificationHub.Providers.Email
{
    using System.Linq;
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>
    /// Class Smtp email provider.
    /// Implements the <see cref="IEmailProvider" />
    /// </summary>
    /// <seealso cref="IEmailProvider" />
    public class SmtpProvider : IEmailProvider
    {
        private readonly SmtpClient _smtpClient;
        private readonly AppSettings _settings;

        /// <summary>Gets or sets the name for the implementor of the INamedInstance interface.</summary>
        /// <value>The name of this instance.</value>
        public string Name { get; set; }

        /// <summary>Initializes a new instance of the <see cref="SmtpProvider" /> class.</summary>
        /// <param name="smtpClient">The configuration.</param>
        /// <param name="settings">The settings.</param>
        public SmtpProvider(SmtpClient smtpClient, AppSettings settings)
        {
            _smtpClient = smtpClient;
            _settings = settings;
        }

        /// <summary>Sends the specified email.</summary>
        /// <param name="email">The email to send.</param>
        public void Send(EmailMessage email)
        {
            SendAsync(email).GetAwaiter().GetResult();
        }

        /// <summary>send as an asynchronous operation.</summary>
        /// <param name="email">The email to send.</param>
        /// <returns>Task.</returns>
        public async Task SendAsync(EmailMessage email)
        {
            try
            {
                var mail = CreateEmailMessage(email);
                await _smtpClient.SendMailAsync(mail);
            }
            catch
            {
                //log an error message or throw an exception, or both.
                throw;
            }
        }

        /// <summary>Creates the email message.</summary>
        /// <param name="message">The message.</param>
        /// <returns>System.Net.Mail.MailMessage.</returns>
        private MailMessage CreateEmailMessage(EmailMessage message)
        {
            var email = new MailMessage {
                From = new MailAddress(_settings.SmtpUsername),
                Subject = message.Subject,
                IsBodyHtml = true,
                Body = message.Content
            };

            foreach (var to in message.To)
                email.Bcc.Add(to);
            
            if (message.Attachments != null && message.Attachments.Any())
            {
                foreach (var file in message.Attachments)
                {
                    // Create  the file attachment for this email message.
                    Attachment data = new Attachment(file.OpenReadStream(), file.ContentType);

                    // Add the file attachment to this email message.
                    email.Attachments.Add(data);
                }
            }

            return email;
        }
    }
}
