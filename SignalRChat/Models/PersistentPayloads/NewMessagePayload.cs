namespace SignalRChat.Models.PersistentPayloads
{
    public class NewMessagePayload : Payload
    {
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}