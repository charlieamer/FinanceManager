using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManager.Controllers
{
	public class AccountController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return new string[] {
					"/account/create",
					"/account/details"
				};
			}
		}

		public ActionResult Create ()
		{
			return View ();
		}

		[HttpPost]
		public ActionResult Create (Account acc)
		{
			if (ModelState.IsValid) {
				// Set created account's user id to current logged user
				acc.UserID = GetSessionUser ().UserID;
				context.Accounts.Add (acc);
				context.SaveChanges ();

				InvalidateUser ();

				return RedirectToHome ();
			}
			return View (acc);
		}

		private Account account;

		public ActionResult Details (int id)
		{
			return View (account);
		}

		protected override void OnResultExecuting (ResultExecutingContext filterContext)
		{
			base.OnResultExecuting (filterContext);
			if (Request.Params.AllKeys.Contains ("id")) {
				int id = Request.Params ["id"] as int;
				Account acc = context.Accounts
					.Include ("Transactions")
					.Where (a => a.AccountID == id)
					.FirstOrDefault ();
				if (acc != null && acc.UserID != GetSessionUserID ())
					filterContext.Cancel = true;
				else
					account = acc;
			}
		}
	}
}
