namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreatePushNotification
    {
        public string EventName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
