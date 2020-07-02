namespace Cloud.Core.NotificationHub.Models.DTO
{
    public class CreatePush
    {
        public string EventName { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
