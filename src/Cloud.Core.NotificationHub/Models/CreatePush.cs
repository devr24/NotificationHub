namespace Cloud.Core.NotificationHub.Models.DTO
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>Create push event.</summary>
    public class CreatePush
    {
        /// <summary>Event name to push for the subscribers.</summary>
        /// <example>LoginEvent</example>
        [Required]
        public string EventName { get; set; }

        /// <summary>Push notification title.</summary>
        /// <example>New Login!</example>
        [Required]
        public string Title { get; set; }

        /// <summary>Push notification body to display.</summary>
        /// <example>You have logged in from another device.</example>
        [Required]
        public string Body { get; set; }
    }
}
