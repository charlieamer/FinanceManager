﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace FinanceManager.Controllers
{
	public class HomeController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return null;
			}
		}

		public ActionResult Index ()
		{
			if (IsLoggedIn ())
				return View ("LoggedIndex");
			else
				return View ("GuestIndex");
		}
	}
}

