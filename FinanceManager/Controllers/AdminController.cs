using System;
using FinanceManager.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace FinanceManager
{
	public class AdminController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return new string[] {
					"/admin",
					"/admin/index",
					"/admin/seed",
					"/admin/users",
					"/admin/deleteuser"
				};
			}
		}

		public ActionResult Index ()
		{
			return View ();
		}

		public ActionResult Seed ()
		{
			context.Seed ();
			SessionLogout ();
			return RedirectToHome ();
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			base.OnActionExecuting (filterContext);
			if (IsLoggedIn () && !SessionUser.Admin)
				filterContext.Result = ErrorResult ("Admin privilages required", 403);
		}

		private ActionResult RedirectToAdminHome ()
		{
			return RedirectToAction ("Index");
		}

		public ActionResult Users ()
		{
			return View (context.Users.Include ("Categories").Include ("Accounts").Include ("Image").ToList ());
		}

		public ActionResult DeleteUser (int? id)
		{
			User user = context.Users.Find (id);
			if (user != null)
				user.Delete (context);
			else
				return ErrorResult ("User not found", 404);
			return Redirect ("/admin/users");
		}

		public ActionResult AdminToggle (int? id)
		{
			User user = context.Users.Find (id);
			if (user != null) {
				user.Admin = !user.Admin;
				context.SaveChanges ();
			} else {
				return ErrorResult ("User not found", 404);
			}
			return Redirect ("/admin/users");
		}
	}
}

