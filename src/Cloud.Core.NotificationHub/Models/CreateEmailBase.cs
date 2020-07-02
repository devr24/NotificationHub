using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.DTO;

namespace Cloud.Core.NotificationHub.Models
{
    public class CreateEmailBase
    {
        private List<ResourceLink> _links = new List<ResourceLink>();

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

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>The links.</value>
        public List<ResourceLink> Links { get => _links; set { _links = value ?? new List<ResourceLink>(); } }
    }
}
