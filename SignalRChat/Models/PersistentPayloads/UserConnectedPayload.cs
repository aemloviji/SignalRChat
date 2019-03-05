using System.Collections.Generic;

namespace SignalRChat.Models.PersistentPayloads
{
    public class UserConnectedPayload : Payload
    {
        public List<User> Users { get; set; }
    }
}