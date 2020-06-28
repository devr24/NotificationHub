using System;
using System.ComponentModel.DataAnnotations;

namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class ResourceLink
    {
        public string Name { get; set; }
        public Uri Link { get; set; }
    }
}
