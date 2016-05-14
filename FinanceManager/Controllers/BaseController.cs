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

		private static SortedSet<int> invalidatedUsers = new SortedSet<int> ();

		protected bool IsLoggedIn ()
		{
			return GetSessionUser () != null;
		}

		protected User GetSessionUser ()
		{
			User user = Session [Strings.SESSION_USER] as User;
			if (user != null) {
				if (invalidatedUsers.Contains (user.UserID)) {
					SessionLogin (user);
					invalidatedUsers.Remove (user.UserID);
				}
			}
			return user;
		}

		protected void SessionLogin (User user)
		{
			if (user != null)
				user = context.Users
					.Include ("Image")
					.Include ("Accounts")
					.FirstOrDefault (u => u.UserID == user.UserID);
			Session [Strings.SESSION_USER] = user;
		}

		protected void SessionLogout ()
		{
			if (IsLoggedIn ())
				Session [Strings.SESSION_USER] = null;
		}

		protected override void OnResultExecuting (ResultExecutingContext filterContext)
		{
			base.OnResultExecuting (filterContext);
			if (AuthenticatedRoutes != null) {
				string route = Request.Path;
				foreach (var aroute in AuthenticatedRoutes) {
					if (route.ToLower () == aroute.ToLower () && !IsLoggedIn ()) {
						filterContext.Cancel = true;
						return;
					}
				}
			}
			ViewData [Strings.SESSION_USER] = GetSessionUser ();
		}

		public static void InvalidateUser (int UserID)
		{
			invalidatedUsers.Add (UserID);
		}

		public static void InvalidateUser (User user)
		{
			if (user != null)
				InvalidateUser (user.UserID);
		}

		protected void InvalidateUser ()
		{
			InvalidateUser (GetSessionUser ());
		}
	}
}