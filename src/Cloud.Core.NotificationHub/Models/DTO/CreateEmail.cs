using System;
using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    /// <summary>
    /// Class CreateEmail.
    /// </summary>
    public class CreateEmail : CreateEmailBase
    {
        private List<IFormFile> _attachments = new List<IFormFile>();

        /// <summary>Gets or sets the email attachments.</summary>
        /// <value>The attachments.</value>
        public List<IFormFile> Attachments { get => _attachments; set { _attachments = value ?? new List<IFormFile>(); } }

        public static implicit operator EmailMessage(CreateEmail source)
        {
            return new EmailMessage
            {
                Content = source.Content,
                IsPlainText = source.IsPlainText,
                Links = source.Links,
                Subject = source.Subject,
                Attachments = source.Attachments,
                TemplateName = source.TemplateName,
                To = source.To
            };
        }
    }

    public class CreateEmailEvent : CreateEmailBase
    {
        public List<Guid> AttachmentIds { get; set; }

        public static implicit operator EmailEvent(CreateEmailEvent source)
        {
            return new EmailEvent
            {
                Content = source.Content,
                IsPlainText = source.IsPlainText,
                Links = source.Links,
                Subject = source.Subject,
                Provider = source.Provider,
                TemplateName = source.TemplateName,
                To = source.To,
                AttachmentIds = source.AttachmentIds
            };
        }
    }

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
