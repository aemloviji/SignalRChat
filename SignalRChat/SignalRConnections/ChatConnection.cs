using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SignalRChat.Models;
using SignalRChat.Models.PersistentClientRequests;
using SignalRChat.Models.PersistentPayloads;

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
            if (commonClientRequestData.Type == RequestTypes.UserConnectedRequest)
            {
                UpdateUserUserName(connectionId, clientRequest);
            }

            var payload = GeneratePayload(connectionId, commonClientRequestData.Type, clientRequest);
            if (commonClientRequestData.Type == RequestTypes.UserConnectedRequest)
            {
                return Connection.Broadcast(payload, connectionId);
            }
            else if (commonClientRequestData.Type == RequestTypes.GetListOfUsersRequest)
            {
                return Connection.Send(connectionId, payload);
            }

            return Connection.Broadcast(payload);
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
            if (clientRequestType == RequestTypes.UserConnectedRequest)
            {
                request = JsonConvert.DeserializeObject<UserConnectedRequest>(data);
            }
            if (clientRequestType == RequestTypes.GetListOfUsersRequest)
            {
                request = DeserializeClientRequestData(data);
            }
            else if (clientRequestType == RequestTypes.NewMessageRequest)
            {
                request = JsonConvert.DeserializeObject<NewMessageRequest>(data);
            }

            return request;
        }

        private object GeneratePayload(string connectionId, string clientRequestType, object data)
        {
            object payload = null;
            if (clientRequestType == RequestTypes.UserConnectedRequest)
            {
                payload = GenerateUserConnectedPayload(connectionId, data);
            }
            else if (clientRequestType == RequestTypes.GetListOfUsersRequest)
            {
                payload = GenerateListOfUserPayload(connectionId);
            }
            else if (clientRequestType == RequestTypes.NewMessageRequest)
            {
                payload = GenerateNewMessagePayload(data);
            }

            return payload;
        }

        private ListOfUserPayload GenerateListOfUserPayload(string connectionId)
        {
            var payload = new ListOfUserPayload()
            {
                Type = nameof(ListOfUserPayload)
            };
            payload.Users.AddRange(GetUsersExcept(connectionId));

            return payload;
        }

        private static IEnumerable<User> GetUsersExcept(string connectionId)
        {
            return _connectedUsers.Where(u => ConnectionIdsAreDifferent(u.ConnectionId, connectionId));
        }

        private static bool ConnectionIdsAreDifferent(string userConnectionId, string connectionId)
        {
            return userConnectionId != connectionId;
        }

        private NewMessagePayload GenerateNewMessagePayload(object data)
        {
            var obj = (NewMessageRequest)data;
            var payload = new NewMessagePayload()
            {
                Type = nameof(NewMessagePayload),
                UserName = obj.UserName,
                Message = obj.Message
            };

            return payload;
        }

        private UserConnectedPayload GenerateUserConnectedPayload(string connectionId, object data)
        {
            var obj = (UserConnectedRequest)data;
            var payload = new UserConnectedPayload()
            {
                Type = nameof(UserConnectedPayload),
                User = new User() { ConnectionId = connectionId, Name = obj.Username }
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
            var user = _connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
            if (HasUser(user))
            {
                _connectedUsers.Remove(user);
            }
        }

        private bool HasUser(User user)
        {
            return user != null;
        }
    }
}