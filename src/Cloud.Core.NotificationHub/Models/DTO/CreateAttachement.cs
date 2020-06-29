using System;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateAttachment
    {
        public IFormFile Content { get; set; }
        public bool IsPublic { get; set; }
        public string ContentText { get; set; }

        [JsonIgnore]
        internal Stream ContentStream => 
            ContentText.IsNullOrEmpty() ?
           Content.OpenReadStream() : ContentText.ConvertToStream(Encoding.UTF8);
    }
}
