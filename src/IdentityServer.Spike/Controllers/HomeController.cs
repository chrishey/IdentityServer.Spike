using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

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

    }
}
