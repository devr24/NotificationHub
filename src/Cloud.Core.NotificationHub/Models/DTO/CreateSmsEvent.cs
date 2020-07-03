using System;
using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateSmsEvent : CreateSmsBase
    {
        private List<Guid> _attachmentIds = new List<Guid>();

        public List<Guid> AttachmentIds { get => _attachmentIds; set { _attachmentIds = value ?? new List<Guid>(); } }

        public static implicit operator SmsEvent(CreateSmsEvent source)
        {
            return new SmsEvent
            {
                Text = source.Text,
                AttachmentIds = source.AttachmentIds,
                Links = source.Links,
                Provider = source.Provider,
                To = source.To
            };
        }
    }
}
