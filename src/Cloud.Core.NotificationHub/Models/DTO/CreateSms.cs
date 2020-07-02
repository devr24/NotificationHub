using System.Collections.Generic;
using Cloud.Core.NotificationHub.Providers;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    /// <summary>
    /// Class CreateSms
    /// </summary>
    public class CreateSms : CreateSmsBase
    {
        private List<IFormFile> _attachments = new List<IFormFile>();

        /// <summary>Gets or sets the sms attachments.</summary>
        /// <value>The attachments.</value>
        public List<IFormFile> Attachments
        {
            get => _attachments;
            set { _attachments = value ?? new List<IFormFile>(); }
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
