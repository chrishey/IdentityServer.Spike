using System.Security.Claims;
using System.Web.Mvc;
using Thinktecture.IdentityModel.Mvc;

namespace IdentityServer.Spike.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
		
		[Authorize]
	    public ActionResult About()
	    {
			return View((User as ClaimsPrincipal).Claims);
	    }

		[ResourceAuthorize("Read", "ContactDetails")]
	    public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page";

		    return View();
	    }

		[ResourceAuthorize("Write", "ContactDetails")]
		[HandleForbidden]
	    public ActionResult UpdateContact()
		{
			ViewBag.Message = "Update your contact details";

			return View();
		}
    }
}
