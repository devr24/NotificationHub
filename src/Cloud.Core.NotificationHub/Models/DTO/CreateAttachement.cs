using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateAttachement
    {
        public IFormFile Content { get; set; }
        public bool IsPublic { get; set; }
    }
}
