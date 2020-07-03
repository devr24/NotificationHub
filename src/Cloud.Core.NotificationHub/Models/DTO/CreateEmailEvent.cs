using System;
using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateEmailEvent : CreateEmailBase
    {
        private List<Guid> _attachmentIds = new List<Guid>();

        public List<Guid> AttachmentIds { get => _attachmentIds; set { _attachmentIds = value ?? new List<Guid>(); } }

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
}
