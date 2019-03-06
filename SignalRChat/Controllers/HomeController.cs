using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using SignalRChat.SignalRHubs;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            SendHubMessage($"Someone navigated to {nameof(Index)}");
            return View();
        }


        public ActionResult HubChat()
        {
            SendHubMessage($"Someone navigated to {nameof(HubChat)}");
            return View();
        }

        public ActionResult PersistentChat()
        {
            SendHubMessage($"Someone navigated to {nameof(PersistentChat)}");
            return View();
        }

        //It sends push notifications using IHubContext.
        //To test it open Index page in one tab, and from another pages try to navigate inside website.
        //First tab will show each user iteraction. It is just for test purposes. And can be enhanced if it will be needed
        private void SendHubMessage(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<PushHub>();
            hubContext.Clients.All.showPushNotification(message);
        }
    }
}