namespace Cloud.Core.NotificationHub.Providers.Email
{
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Threading.Tasks;

    /// <summary>Smtp configuration class.</summary>
    public class SmtpConfig
    {
        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server.
        /// </summary>
        /// <value>The SMTP server.</value>
        public string SmtpServer { get; set; }
        
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }
    }

    /// <summary>
    /// Class Smtp email provider.
    /// Implements the <see cref="IEmailProvider" />
    /// </summary>
    /// <seealso cref="IEmailProvider" />
    public class SmtpProvider : IEmailProvider
    {
        /// <summary>Gets the configuration.</summary>
        /// <value>The configuration.</value>
        public SmtpConfig Configuration { get; }

        /// <summary>Gets or sets the name for the implementor of the INamedInstance interface.</summary>
        /// <value>The name of this instance.</value>
        public string Name { get; set; }

        /// <summary>Initializes a new instance of the <see cref="SmtpProvider"/> class.</summary>
        /// <param name="config">The configuration.</param>
        public SmtpProvider(SmtpConfig config)
        {
            Configuration = config;
        }

        /// <summary>Sends the specified email.</summary>
        /// <param name="email">The email to send.</param>
        public void Send(EmailMessage email)
        {
            using var client = new SmtpClient(Configuration.SmtpServer, Configuration.Port)
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };
            
            try
            {
                var mail = CreateEmailMessage(email);
                client.Send(mail);
            }
            catch
            {
                //log an error message or throw an exception or both.
                throw;
            }
        }

        /// <summary>send as an asynchronous operation.</summary>
        /// <param name="email">The email to send.</param>
        /// <returns>Task.</returns>
        public async Task SendAsync(EmailMessage email)
        {
            using var client = new SmtpClient(Configuration.SmtpServer, Configuration.Port)
            {
                Credentials = CredentialCache.DefaultNetworkCredentials
            };
            try
            {
                var mail = CreateEmailMessage(email);
                await client.SendMailAsync(mail);
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
                From = new MailAddress(Configuration.From),
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
