namespace SignalRChat.Models.PersistentClientRequests
{
    public class NewMessageRequest : ClientRequest
    {
        public string Message { get; set; }
    }
}