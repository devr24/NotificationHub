namespace Cloud.Core.NotificationHub.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Cloud.Core.NotificationHub.Models.DTO;

    /// <summary>
    /// Class Sms Message.
    /// </summary>
    public class SmsMessage
    {
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

        internal string FullContent => $"{Text}\n{string.Join("\n", Links.Select(l => $"{l.Name}: {l.Link}"))}";
    }
}
