namespace Cloud.Core.NotificationHub.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Notification;
    using Microsoft.AspNetCore.Http;

    /// <summary>Create Email.</summary>
    public class CreateEmail
    {
        private List<IFormFile> _attachments = new List<IFormFile>();

        /// <summary>Gets or sets the email provider to use while sending.</summary>
        /// <example>EmailProviders.SmtpProvider</example>
        public EmailProviders? Provider { get; set; } = EmailProviders.SmtpProvider;

        /// <summary>Recipient list for the email (sent as blind carbon copy).</summary>
        /// <example>
        /// - robert.mccabe@outlook.com
        /// </example>
        [Required]
        public List<string> To { get; set; }

        /// <summary>Subject for the email being sent.</summary>
        /// <example>Test subject</example>
        [Required]
        public string Subject { get; set; }

        /// <summary>Gets or sets the email content.</summary>
        /// <example><h1>This is an example email!</h1></example>
        [Required]
        public string Content { get; set; }

        /// <summary>The email attachments.</summary>
        public List<IFormFile> Attachments { get => _attachments; set { _attachments = value ?? new List<IFormFile>(); } }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CreateEmail"/> to <see cref="EmailMessage"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator EmailMessage(CreateEmail source)
        {
            var mail = new EmailMessage
            {
                Subject = source.Subject,
                Content = source.Content
            };
            mail.Attachments.AddRange(source.Attachments.Select(a => new EmailAttachment { Name = a.FileName, Content = a.OpenReadStream(), ContentType = a.ContentType }));
            mail.To.AddRange(source.To);

            return mail;
        }
    }

    /// <summary>Create Template Email.</summary>
    public class CreateTemplateEmail
    {
        private List<IFormFile> _attachments = new List<IFormFile>();

        /// <summary>Gets or sets the email provider to use while sending.</summary>
        /// <example>EmailProviders.SmtpProvider</example>
        public EmailProviders? Provider { get; set; } = EmailProviders.SmtpProvider;

        /// <summary>Recipient list for the email (sent as blind carbon copy).</summary>
        /// <example>
        /// - robert.mccabe@outlook.com
        /// </example>
        [Required]
        public List<string> To { get; set; }

        /// <summary>Subject for the email being sent.</summary>
        /// <example>Test subject</example>
        [Required]
        public string Subject { get; set; }

        /// <summary>The template identifier to use.</summary>
        internal string TemplateId { get; set; }

        /// <summary>Gets or sets the email content.</summary>
        [Required]
        public string TemplateContent { get; set; }

        /// <summary>The email attachments.</summary>
        public List<IFormFile> Attachments { get => _attachments; set { _attachments = value ?? new List<IFormFile>(); } }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CreateTemplateEmail"/> to <see cref="EmailTemplateMessage"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator EmailTemplateMessage(CreateTemplateEmail source)
        {
            var mail = new EmailTemplateMessage
            {
                Subject = source.Subject,
                TemplateId =  source.TemplateId,
                TemplateObject = source.TemplateContent
            };
            mail.Attachments.AddRange(source.Attachments.Select(a => new EmailAttachment { Name = a.FileName, Content = a.OpenReadStream(), ContentType = a.ContentType }));
            mail.To.AddRange(source.To);

            return mail;
        }
    }
}
