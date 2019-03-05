namespace SignalRChat.Models.PersistentClientRequests
{
    public class NewMessageRequest : ClientRequest
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}