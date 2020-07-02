using System.Collections.Generic;
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
}
