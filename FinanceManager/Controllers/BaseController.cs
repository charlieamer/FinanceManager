using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinanceManager.Controllers
{
    public class BaseController : Controller
    {
		protected ModelContext context = new ModelContext();

		protected bool IsLoggedIn() {
			return GetSessionUser() != null;
		}

		protected User GetSessionUser() {
			return Session [Strings.SESSION_USER] as User;
		}

		protected void SessionLogin(User user) {
			Session [Strings.SESSION_USER] = user;
		}

		protected override void OnResultExecuting (ResultExecutingContext filterContext)
		{
			base.OnResultExecuting (filterContext);
			ViewData [Strings.SESSION_USER] = GetSessionUser ();
		}
    }
}