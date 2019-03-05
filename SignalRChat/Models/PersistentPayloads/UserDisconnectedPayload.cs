using System.Collections.Generic;

namespace SignalRChat.Models.PersistentPayloads
{
    public class UserDisconnectedPayload : Payload
    {
        public string ConnectionId { get; set; }
    }
}