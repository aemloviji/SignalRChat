using System.Collections.Generic;

namespace SignalRChat.Models.PersistentPayloads
{
    public class UserConnectedPayload : Payload
    {
        public User User { get; set; }
    }
}