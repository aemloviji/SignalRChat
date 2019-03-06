using System;
using System.Collections.Generic;

namespace SignalRChat.Models.PersistentPayloads
{
    public class ListOfUserPayload : Payload
    {
        public List<User> Users { get; set; }

        public ListOfUserPayload()
        {
            Users = new List<User>();
        }
    }
}