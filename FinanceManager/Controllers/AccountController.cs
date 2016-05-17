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

		public ActionResult Details ()
		{
			if (account == null)
				return RedirectToHome ();
			return View (account);
		}

		public ActionResult Delete ()
		{
			if (account != null)
				account.Delete (context);
			return RedirectToHome ();
		}

		[HttpGet]
		public ActionResult CreateTransaction (int? id, Transaction transaction)
		{
			if (transaction == null) {
				transaction = new Transaction ();
				transaction.TransactionTime = DateTime.Now;
			}
			return View (transaction);
		}

		[HttpPost]
		public ActionResult CreateTransaction (Transaction transaction)
		{
			ActionResult res = CheckForAccountPermissions (transaction.AccountID);
			if (res != null)
				return res;
			if (transaction.TransactionTime > DateTime.Now)
				ModelState.AddModelError ("TransactionTime", "Transaction time cannot be in future");
			if (transaction.Amount < 0)
				ModelState.AddModelError ("Amount", "Amount cannot be negative");
			if (ModelState.IsValid) {
				try {
					account.AddTransaction (transaction, context);
				} catch (ArgumentException ex) {
					ModelState.AddModelError ("Amount", ex.Message);
				}
			}
			if (ModelState.IsValid) {
				return Redirect ("/account/details?id=" + transaction.AccountID);
			}
			return CreateTransaction (transaction.AccountID, transaction);
		}

		private ActionResult CheckForAccountPermissions (int id)
		{
			Account acc = context.Accounts
				.Include ("Transactions")
				.Where (a => a.AccountID == id)
				.FirstOrDefault ();
			account = acc;
			ViewData ["account"] = acc;
			if (acc != null && acc.UserID != GetSessionUserID ())
				return ErrorResult ("You don't have permission to edit this account", 403);
			else if (acc == null)
				return ErrorResult ("Account does not exist", 404);
			return null;
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			base.OnActionExecuting (filterContext);
			account = null;
			if (Request.Params.AllKeys.Contains ("id")) {
				int id;
				if (int.TryParse (Request.Params ["id"], out id)) {
					ActionResult res = CheckForAccountPermissions (id);
					if (res != null)
						filterContext.Result = res;
				} else {
					filterContext.Result =
						ErrorResult ("Invalid id", 403);
				}
			}
		}
	}
}
