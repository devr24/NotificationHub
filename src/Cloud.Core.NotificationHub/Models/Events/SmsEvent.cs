using System;
using System.Collections.Generic;
using System.Linq;
using Cloud.Core.NotificationHub.Models.DTO;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.Events
{
    /// <summary>
    /// Class CreateSms
    /// </summary>
    public class SmsEvent
    {
        /// <summary>Gets or sets the sms provider to use while sending.</summary>
        /// <value>The sms provider.</value>
        public SmsProviders? Provider { get; set; } = SmsProviders.DummySmsProvider;

        /// <summary>Gets or sets the recipient list.</summary>
        /// <value>To.</value>
        public List<string> To { get; set; }

        /// <summary>Gets or sets the message text.</summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>The links.</value>
        public List<ResourceLink> Links { get; set; }

        /// <summary>Gets or sets the email attachments.</summary>
        /// <value>The attachments.</value>
        public List<Guid> AttachmentIds { get; set; }

        public static implicit operator SmsMessage(SmsEvent source)
        {
            var sms =  new SmsMessage
            {
                Text = source.Text,
                To = source.To
            };
            sms.Links.AddRange(source.Links.Select(l => new SmsLink { Title = l.Name, Link = l.Link }));
            return sms;
        }
    }
}
