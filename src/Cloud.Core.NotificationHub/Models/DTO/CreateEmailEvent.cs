using System;
using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;

namespace Cloud.Core.NotificationHub.Models.DTO
{
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
}
