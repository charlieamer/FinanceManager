using System;
using FinanceManager.Controllers;
using System.Web.Mvc;

namespace FinanceManager
{
	public class AdminController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return new string[] {
					"/admin",
					"/admin/index",
					"/admin/seed"
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
	}
}

