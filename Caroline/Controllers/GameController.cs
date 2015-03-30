using System.Threading.Tasks;
using System.Web.Mvc;
using Caroline.Api;

namespace Caroline.Controllers
{
    public class GameController : Controller
    {
        public async Task<ActionResult> Index()
        {
            await AnonymousProfileApi.GenerateAnonymousProfileIfNotAuthenticated(HttpContext);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}