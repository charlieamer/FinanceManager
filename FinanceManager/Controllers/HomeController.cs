using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace FinanceManager.Controllers
{
	public class HomeController : BaseController
	{
		public ActionResult Index ()
		{
			if (IsLoggedIn ())
				return View ("LoggedIndex");
			else
				return View ("GuestIndex");
		}
	}
}

