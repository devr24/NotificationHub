namespace Cloud.Core.NotificationHub.Models
{
    using System.Collections.Generic;
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

        public static implicit operator SmsMessage(CreateSms sms)
        {
            var smsMessage = new SmsMessage
            {
                To = sms.To,
                Text = sms.Text
            };

            return smsMessage;

        }
    }
}
