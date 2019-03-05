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
            var commonClientRequestData = DeserializeClientRequestData(data);
            var clientRequest = DeserializeClientRequest(commonClientRequestData.Type, data);
            if (commonClientRequestData.Type == nameof(UserConnectedRequest))
            {
                UpdateUserUserName(connectionId, clientRequest);
            }

            return Connection.Broadcast(GeneratePayload(commonClientRequestData.Type, clientRequest));
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            RemoveUser(connectionId);
            var payload = CreateUserDisconnectedPayload(connectionId);

            return Connection.Broadcast(payload, connectionId);
        }

        private void UpdateUserUserName(string connectionId, object clientRequest)
        {
            var user = FindUserWithEmptyUserName(connectionId);
            if (HasUser(user))
            {
                user.Name = ((UserConnectedRequest)clientRequest).Username;
            }
        }

        private User FindUserWithEmptyUserName(string connectionId)
        {
            return _connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId && string.IsNullOrEmpty(u.Name));
        }

        private void AddUserToList(string connectionId)
        {
            _connectedUsers.Add(CreateUser(connectionId));
        }

        private User CreateUser(string connectionId)
        {
            return new User()
            {
                ConnectionId = connectionId
            };
        }

        private ClientRequest DeserializeClientRequestData(string data)
        {
            return JsonConvert.DeserializeObject<ClientRequest>(data);
        }

        private object DeserializeClientRequest(string clientRequestType, string data)
        {
            object request = null;
            if (clientRequestType == nameof(UserConnectedRequest))
            {
                request = JsonConvert.DeserializeObject<UserConnectedRequest>(data);
            }
            else if (clientRequestType == nameof(NewMessageRequest))
            {
                request = JsonConvert.DeserializeObject<NewMessageRequest>(data);
            }

            return request;
        }

        private object GeneratePayload(string clientRequestType, object data)
        {
            object payload = null;
            if (clientRequestType == nameof(UserConnectedRequest))
            {
                payload = GenerateUserConnectedPayload(data);
            }
            else if (clientRequestType == nameof(NewMessageRequest))
            {
                payload = GenerateNewMessagePayload(data);
            }

            return payload;
        }

        private object GenerateNewMessagePayload(object data)
        {
            var obj = (NewMessageRequest)data;
            object payload = new NewMessagePayload()
            {
                Type = nameof(NewMessagePayload),
                UserName = obj.UserName,
                Message = obj.Message
            };

            return payload;
        }

        private object GenerateUserConnectedPayload(object data)
        {
            var obj = (UserConnectedRequest)data;
            var payload = new UserConnectedPayload()
            {
                Type = nameof(UserConnectedPayload),
                Users = new List<User>()
            };

            return payload;
        }

        private UserDisconnectedPayload CreateUserDisconnectedPayload(string connectionId)
        {
            return new UserDisconnectedPayload()
            {
                Type = nameof(UserDisconnectedPayload),
                ConnectionId = connectionId
            };
        }

        private void RemoveUser(string connectionId)
        {
            _connectedUsers.Add(CreateUser(connectionId));
        }

        private bool HasUser(User user)
        {
            return user != null;
        }
    }
}