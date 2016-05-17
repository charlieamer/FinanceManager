using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManager.Controllers
{

	public abstract class BaseController : Controller
	{
		protected abstract string[] AuthenticatedRoutes { get; }

		protected ModelContext context = new ModelContext ();

		protected User SessionUser { get; private set; }

		protected bool IsLoggedIn ()
		{
			return Session [Strings.SESSION_USER] != null;
		}

		protected ActionResult ErrorResult (string text, int status)
		{
			ViewData ["text"] = text;
			Response.StatusCode = status;
			return View ("Error");
		}

		protected ActionResult RedirectToHome ()
		{
			return RedirectToAction ("Index", "Home");
		}

		protected int GetSessionUserID ()
		{
			if (IsLoggedIn ())
				return SessionUser.UserID;
			else
				return -1;
		}

		protected void SessionLogin (User user)
		{
			if (user != null)
				Session [Strings.SESSION_USER] = user.UserID;
			else
				Session [Strings.SESSION_USER] = null;
		}

		private void LoadSessionUser ()
		{
			int? user_id = Session [Strings.SESSION_USER] as int?;
			if (user_id != null)
				SessionUser = context.Users
					.Include ("Image")
					.Include ("Accounts")
					.FirstOrDefault (u => u.UserID == user_id);
		}

		protected void SessionLogout ()
		{
			if (IsLoggedIn ())
				Session [Strings.SESSION_USER] = null;
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			base.OnActionExecuting (filterContext);
			if (AuthenticatedRoutes != null) {
				string route = Request.Path;
				string r = route;
				while (r.EndsWith ("/"))
					r = r.Remove (route.Length - 1);
				foreach (var aroute in AuthenticatedRoutes) {
					if (r.ToLower () == aroute.ToLower () && !IsLoggedIn ()) {
						filterContext.Result = ErrorResult ("Action requires login", 403);
						return;
					}
				}
			}
			LoadSessionUser ();
			if (SessionUser != null)
				ViewData [Strings.SESSION_USER] = SessionUser;
		}
	}
}