using System.Collections.Generic;
using System.Linq;
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
            var mail = new EmailMessage
            {
                Content = source.Content,
                IsPlainText = source.IsPlainText,
                Subject = source.Subject,
                TemplateName = source.TemplateName
            };
            mail.Attachments.AddRange(source.Attachments.Select(a => new EmailAttachment { Name = a.FileName, Content = a.OpenReadStream(), ContentType = a.ContentType }));
            mail.To.AddRange(source.To.Select(a => new EmailRecipient { Name = a, Address = a}));

            return mail;
        }
    }
}
