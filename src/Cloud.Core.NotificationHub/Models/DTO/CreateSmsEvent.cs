using System;
using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateSmsEvent : CreateSmsBase 
    {
        public List<Guid> AttachmentIds { get; set; }

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
