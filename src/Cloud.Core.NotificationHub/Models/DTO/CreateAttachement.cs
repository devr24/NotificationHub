using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreateAttachment
    {
        [Required]
        public IFormFile File { get; set; }
        public bool IsPublic { get; set; }
    }
}
