namespace SignalRChat.Models.PersistentPayloads
{
    public class NewMessagePayload : Payload
    {
        public string Message { get; set; }
    }
}