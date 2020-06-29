using Cloud.Core.NotificationHub.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Cloud.Core.NotificationHub.Providers
{
    /// <summary>
    /// Class Email Message.
    /// </summary>
    public class EmailMessage
    {
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

        /// <summary>Gets or sets the email attachments.</summary>
        /// <value>The attachments.</value>
        public IFormFileCollection Attachments { get; set; }

        public static implicit operator EmailMessage(CreateEmail email)
        {
            var emailMessage = new EmailMessage
            {
                To = email.To,
                Subject = email.Subject,
               // Attachments = email.Attachments,
                Content = email.Content,
                TemplateName = email.TemplateName
            };

            return emailMessage;
        }
    }
}
