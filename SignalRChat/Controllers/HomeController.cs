using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HubChat()
        {
            return View();
        }

        public ActionResult PersistentChat()
        {
            return View();
        }
    }
}