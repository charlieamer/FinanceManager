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
					"/account/details",
					"/account/delete"
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
				acc.UserID = SessionUser.UserID;
				context.Accounts.Add (acc);
				context.SaveChanges ();

				return RedirectToHome ();
			}
			return View (acc);
		}

		private Account account;

		public ActionResult Details (int? id)
		{
			return View (account);
		}

		public ActionResult Delete (int? id)
		{
			account.Delete (context);
			return RedirectToHome ();
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			base.OnActionExecuting (filterContext);
			if (Request.Params.AllKeys.Contains ("id")) {
				int id;
				if (int.TryParse (Request.Params ["id"], out id)) {
					Account acc = context.Accounts
						.Include ("Transactions")
						.Where (a => a.AccountID == id)
						.FirstOrDefault ();
					if (acc != null && acc.UserID != GetSessionUserID ())
						filterContext.Result =
							ErrorResult ("You don't have permission to edit this account", 403);
					else if (acc == null)
						filterContext.Result = ErrorResult ("Account does not exist", 404);
					else
						account = acc;
				} else {
					filterContext.Result =
						ErrorResult ("Invalid id", 403);
				}
			}
		}
	}
}
