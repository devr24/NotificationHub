using System.Collections.Generic;
using Cloud.Core.NotificationHub.Models.DTO;

namespace Cloud.Core.NotificationHub.Models
{
    public class CreateSmsBase
    {
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

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>The links.</value>
        public List<ResourceLink> Links { get => _links; set { _links = value ?? new List<ResourceLink>(); } }
    }
}
