using System;
using System.Collections.Generic;
using System.Linq;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;

namespace Cloud.Core.NotificationHub.Models.Events
{
    /// <summary>
    /// Class CreateEmail.
    /// </summary>
    public class EmailEvent
    {
        /// <summary>Gets or sets the email provider to use while sending.</summary>
        /// <value>The email provider.</value>
        public EmailProviders? Provider { get; set; } = EmailProviders.SmtpProvider;

        /// <summary>Gets or sets the recipient list (send as blind carbon copy).</summary>
        /// <value>List of string recipients.</value>
        public List<string> To { get; set; }

        /// <summary>Gets or sets the email subject.</summary>
        /// <value>The email subject.</value>
        public string Subject { get; set; }

        /// <summary>Gets or sets the name of the email template to use.</summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; set; }

        /// <summary>Gets or sets the email content.</summary>
        /// <value>The email content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is plain text.
        /// </summary>
        /// <value><c>true</c> if this instance is plain text; otherwise, <c>false</c>.</value>
        public bool IsPlainText { get; set; }

        public List<ResourceLink> Links { get; set; }

        /// <summary>Gets or sets the email attachments.</summary>
        /// <value>The attachments.</value>
        public List<Guid> AttachmentIds { get; set; }

        public static implicit operator EmailMessage(EmailEvent source)
        {
            var email = new EmailMessage
            {
                Content = source.Content,
                IsPlainText = source.IsPlainText,
                Subject = source.Subject,
                TemplateName = source.TemplateName
            };
            email.To.AddRange(source.To.Select(t => new EmailRecipient { Name = t, Address = t }));
            return email;
        }
    }
}
