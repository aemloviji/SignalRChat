using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SignalRChat.Models;

namespace SignalRChat.SignalRHubs
{
    public class ChatHub : Hub
    {
        private static readonly List<User> _connectedUsers = new List<User>();

        //This method will be called from client when user try to send message
        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        //This method will be called from client when user try to login
        public void Connect(string userName)
        {
            var connectionId = RetreiveConnectionIdFromContext();
            if (UserDoesNotExist(connectionId))
            {
                AddUserToList(userName, connectionId);
                SendUserConnectedSignals(userName, connectionId);
            }
        }

        private void SendUserConnectedSignals(string userName, string connectionId)
        {
            Clients.Caller.onConnected(connectionId, userName, _connectedUsers);
            Clients.AllExcept(connectionId).onNewUserConnected(connectionId, userName);
        }

        //This method will be called automatically when user will close tab or browser
        public override Task OnDisconnected(bool stopCalled)
        {
            HandleOnDisconnetedAction();
            return base.OnDisconnected(stopCalled);
        }


        #region private methods
        private string RetreiveConnectionIdFromContext()
        {
            return Context.ConnectionId;
        }
        private User FindUser(string connectionId)
        {
            return _connectedUsers.FirstOrDefault(u => u.ConnectionId == connectionId);
        }

        private static bool UserDoesNotExist(string connectionId)
        {
            return !_connectedUsers.Any(u => u.ConnectionId == connectionId);
        }

        private void AddUserToList(string userName, string connectionId)
        {
            _connectedUsers.Add(CreateUser(connectionId, userName));
        }

        private User CreateUser(string connectionId, string userName)
        {
            return new User()
            {
                ConnectionId = connectionId,
                Name = userName
            };
        }

        private bool UserIsNotNull(User user)
        {
            return user != null;
        }

        private void RemoveUserFromList(User user)
        {
            _connectedUsers.Remove(user);
        }

        private void HandleOnDisconnetedAction()
        {
            var connectionId = RetreiveConnectionIdFromContext();
            var user = FindUser(connectionId);
            if (UserIsNotNull(user))
            {
                RemoveUserFromList(user);
                SendUserDisconnectedSignalToAllUsers(connectionId);
            }
        }

        private void SendUserDisconnectedSignalToAllUsers(string connectionId)
        {
            Clients.All.onUserDisconnected(connectionId);
        }
        #endregion
    }
}