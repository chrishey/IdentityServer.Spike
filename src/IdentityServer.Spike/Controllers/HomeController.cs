using System.Configuration;
using System.Security.Claims;
using System.Web;
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
			return View(((ClaimsPrincipal) User).Claims);
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

	    public ActionResult Logout()
	    {
		    Request.GetOwinContext().Authentication.SignOut();
		    return Redirect("/");
	    }

		[HttpGet]
	    public ActionResult Login()
	    {
		    return View();
	    }

	    [HttpPost]
	    public ActionResult Login(string username, string password, string returnUrl)
	    {
		    if (username == ConfigurationManager.AppSettings["user"] &&
		        password == ConfigurationManager.AppSettings["password"])
		    {
			    var claims = new[]
			    {
					new Claim("name", "Chris"),
 					new Claim("role", "admin") 
			    };

				var identity = new ClaimsIdentity(claims, ConfigurationManager.AppSettings["AuthType"]);
			    Request.GetOwinContext().Authentication.SignIn(identity);
				return Redirect(returnUrl);
		    }

		    return View();
	    }
    }
}
