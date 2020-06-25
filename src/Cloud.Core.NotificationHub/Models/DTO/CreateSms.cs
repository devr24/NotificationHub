using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    /// <summary>
    /// Class CreateSms
    /// </summary>
    public class CreateSms
    {
        /// <summary>Gets or sets the sms provider to use while sending.</summary>
        /// <value>The sms provider.</value>
        public SmsProviders? Provider { get; set; } = SmsProviders.ClickatelProvider;

        /// <summary>Gets or sets the recipient list.</summary>
        /// <value>To.</value>
        [Required]
        public List<string> To { get; set; }

        /// <summary>Gets or sets the message text.</summary>
        /// <value>The text.</value>
        [Required]
        public string Text { get; set; }
    }
}
