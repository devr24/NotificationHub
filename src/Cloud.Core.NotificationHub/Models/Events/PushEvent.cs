namespace Cloud.Core.NotificationHub.Models.Events
{
    public class PushEvent
    {
        public string EventName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
