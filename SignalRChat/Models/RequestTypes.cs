using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class RequestTypes
    {
        public const string UserConnectedRequest = "UserConnectedRequest";
        public const string NewMessageRequest = "NewMessageRequest";
        public const string GetListOfUsersRequest = "GetListOfUsersRequest";
    }
}