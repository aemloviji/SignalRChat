using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using SignalRChat.Models;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly List<User> _users = new List<User>();

        public void Send(string name, string message)
        {
            Clients.All.addMessage(name, message);
        }

        public void Connect(string userName)
        {
            var connectionId = Context.ConnectionId;

            if (HasNotUser(connectionId))
            {
                AddUserToList(userName, connectionId);

                Clients.Caller.onConnected(connectionId, userName, _users);
                Clients.AllExcept(connectionId).onNewUserConnected(connectionId, userName);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = Context.ConnectionId;
            var user = _users.FirstOrDefault(u => u.ConnectionId == connectionId);

            if (UserIsNotNull(user))
            {
                _users.Remove(user);
                Clients.All.onUserDisconnected(connectionId, user.Name);
            }

            return base.OnDisconnected(stopCalled);
        }

        private static bool HasNotUser(string connectionId)
        {
            return !_users.Any(u => u.ConnectionId == connectionId);
        }

        private static void AddUserToList(string userName, string connectionId)
        {
            _users.Add(CreateUser(connectionId, userName));
        }

        private static User CreateUser(string connectionId, string userName)
        {
            return new User()
            {
                ConnectionId = connectionId,
                Name = userName
            };
        }

        private static bool UserIsNotNull(User user)
        {
            return user != null;
        }
    }
}