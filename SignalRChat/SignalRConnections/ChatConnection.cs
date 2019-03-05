using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SignalRChat.Models;
using SignalRChat.Models.PersistentClientRequests;
using SignalRChat.Models.PersistentPayloads;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRChat.SignalRConnections
{
    public class ChatConnection : PersistentConnection
    {
        private static readonly List<User> _connectedUsers = new List<User>();

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            AddUserToList(connectionId);
            return Task.CompletedTask;
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            var clientRequest = ParseClientRequest(data);

            if (clientRequest.GetType() == typeof(UserConnectedRequest))
            {
                var user = _connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId && string.IsNullOrEmpty(u.Name));
                if (UserIsNotNull(user))
                {
                    //TODO make it generic
                    user.Name = ((UserConnectedRequest)clientRequest).Username;
                }
            }

            return Connection.Broadcast(clientRequest);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            RemoveUser(connectionId);
            return Connection.Broadcast(CreateUserDisconnectedPayload(connectionId), connectionId);
        }

        private static void AddUserToList(string connectionId)
        {
            _connectedUsers.Add(CreateUser(connectionId));
        }

        private static User CreateUser(string connectionId)
        {
            return new User()
            {
                ConnectionId = connectionId
            };
        }


        private static object ParseClientRequest(string data)
        {
            object request = null;

            ClientRequest clientRequest = JsonConvert.DeserializeObject<ClientRequest>(data);
            switch (clientRequest.Type)
            {
                case nameof(UserConnectedRequest):
                    request = JsonConvert.DeserializeObject<UserConnectedPayload>(data);
                    break;
                case nameof(NewMessageRequest):
                    request = JsonConvert.DeserializeObject<NewMessageRequest>(data);
                    break;
            }

            return request;
        }

        private static UserConnectedPayload CreateUserConnectedPayload()
        {
            return new UserConnectedPayload()
            {
                Type = nameof(UserConnectedPayload),
                Users = _connectedUsers
            };
        }


        private static UserDisconnectedPayload CreateUserDisconnectedPayload(string connectionId)
        {
            return new UserDisconnectedPayload()
            {
                Type = nameof(UserDisconnectedPayload),
                ConnectionId = connectionId
            };
        }

        private static void RemoveUser(string connectionId)
        {
            _connectedUsers.Add(CreateUser(connectionId));
        }

        private static bool UserIsNotNull(User user)
        {
            return user != null;
        }
    }
}