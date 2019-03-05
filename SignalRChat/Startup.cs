using Microsoft.Owin;
using Owin;
using SignalRChat.SignalRConnections;

[assembly: OwinStartup(typeof(SignalRChat.Startup))]

namespace SignalRChat
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //Map Hub Model
            app.MapSignalR();

            //Map Persistent Model
            app.MapSignalR<ChatConnection>("/chat");
        }
    }
}
