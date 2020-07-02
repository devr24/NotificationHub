using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.Events;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    /// <summary>
    /// Class CreateSms
    /// </summary>
    public class CreateSms
    {
        private List<IFormFile> _attachments = new List<IFormFile>();
        private List<ResourceLink> _links = new List<ResourceLink>();

        /// <summary>Gets or sets the sms provider to use while sending.</summary>
        /// <value>The sms provider.</value>
        public SmsProviders? Provider { get; set; } = SmsProviders.ClickatelProvider;

        /// <summary>Gets or sets the recipient list.</summary>
        /// <value>To.</value>
        public List<string> To { get; set; }

        /// <summary>Gets or sets the message text.</summary>
        /// <value>The text.</value>
        public string Text { get; set; }

        /// <summary>Gets or sets the sms attachments.</summary>
        /// <value>The attachments.</value>
        public List<IFormFile> Attachments { 
            get => _attachments; 
            set { _attachments = value ?? new List<IFormFile>(); } }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>The links.</value>
        public List<ResourceLink> Links { get => _links; set { _links = value ?? new List<ResourceLink>(); } }

        public static implicit operator SmsEvent(CreateSms source)
        {
            return new SmsEvent
            {
                Text = source.Text,
                Attachments = source.Attachments,
                Links = source.Links,
                Provider = source.Provider,
                To = source.To
            };
        }

        public static implicit operator SmsMessage(CreateSms source)
        {
            return new SmsMessage
            {
                Text = source.Text,
                Links = source.Links,
                To = source.To
            };
        }
    }
}
