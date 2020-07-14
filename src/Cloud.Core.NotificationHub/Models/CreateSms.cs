namespace Cloud.Core.NotificationHub.Models.DTO
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Cloud.Core.Notification;
    using Microsoft.AspNetCore.Http;

    /// <summary>Create Sms message.</summary>
    public class CreateSms 
    {
        private List<IFormFile> _attachments = new List<IFormFile>();

        /// <summary>Sms provider to use while sending.</summary>
        /// <example>SmsProviders.ClickatelProvider</example>
        public SmsProviders? Provider { get; set; } = SmsProviders.ClickatelProvider;

        /// <summary>Recipient list of phone numbers, with area codes.</summary>
        /// <example>
        /// +447398225155
        /// </example>
        [Required]
        public List<string> To { get; set; }

        /// <summary>Sms message text to send.</summary>
        /// <example>This is an example sms message.</example>
        [Required]
        public string Text { get; set; }

        /// <summary>List of attachment id's to use when generating the message.</summary>
        public List<IFormFile> Attachments
        {
            get => _attachments;
            set { _attachments = value ?? new List<IFormFile>(); }
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="CreateSms"/> to <see cref="SmsMessage"/>.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator SmsMessage(CreateSms source)
        {
            var sms = new SmsMessage
            {
                Text = source.Text,
                
            };
            sms.To.AddRange(source.To);
            return sms;
        }
    }
}
