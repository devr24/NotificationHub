using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models
{
    /// <summary>
    /// Class Email.
    /// </summary>
    public class CreateEmail
    {
        /// <summary>Gets or sets the email provider to use while sending.</summary>
        /// <value>The email provider.</value>
        public EmailProviders? Provider { get; set; } = EmailProviders.SmtpProvider;

        /// <summary>Gets or sets the recipient list (send as blind carbon copy).</summary>
        /// <value>List of string recipients.</value>
        [Required]
        public List<string> To { get; set; }

        /// <summary>Gets or sets the email subject.</summary>
        /// <value>The email subject.</value>
        [Required]
        public string Subject { get; set; }

        /// <summary>Gets or sets the name of the email template to use.</summary>
        /// <value>The name of the template.</value>
        public string TemplateName { get; set; }

        /// <summary>Gets or sets the email content.</summary>
        /// <value>The email content.</value>
        [Required]
        public string Content { get; set; }

        /// <summary>Gets or sets the email attachments.</summary>
        /// <value>The attachments.</value>
        public IFormFileCollection Attachments { get; set; }
    }
}
