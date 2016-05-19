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
					"/account/delete",
					"/account/edit",
					"/account/edittransaction",
					"/account/createtransaction"
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

		public ActionResult Edit ()
		{
			ViewData ["id"] = account.AccountID;
			return View ("create", account);
		}

		[HttpPost]
		public ActionResult Edit (Account edited)
		{
			ActionResult r = CheckForAccountPermissions (edited.AccountID);
			if (r != null)
				return r;
			if (ModelState.IsValid) {
				account.FromAnother (edited);
				context.SaveChanges ();
				return Redirect ("/account/details?id=" + account.AccountID);
			}
			return Edit ();
		}

		private ActionResult PrepareCreateTransactionView (Transaction transaction)
		{
			int user_id = GetSessionUserID ();
			ViewData ["categories"] = context.Categories.Where (c => c.UserID == user_id).ToList ();
			return View ("createtransaction", transaction);
		}

		private ActionResult PrepareEditTransactionView (Transaction transaction)
		{
			ViewData ["id"] = transaction.TransactionID;
			return PrepareCreateTransactionView (transaction);
		}

		public ActionResult EditTransaction (int? TransactionID)
		{
			Transaction transaction = context.Transactions.Find (TransactionID);
			if (transaction == null)
				return ErrorResult ("Transaction not found", 404);
			ActionResult r = CheckForAccountPermissions (transaction.AccountID);
			if (r != null)
				return r;
			return PrepareEditTransactionView (transaction);
		}

		[HttpPost]
		public ActionResult EditTransaction (Transaction edited)
		{
			Transaction transaction = context.Transactions.Find (edited.TransactionID);
			if (transaction == null)
				return ErrorResult ("Transaction not found", 404);
			ActionResult r = CheckForAccountPermissions (transaction.AccountID);
			if (r != null)
				return r;
			if (ModelState.IsValid) {
				try {
					account.EditTransaction (edited, context);
					return Redirect ("/account/details?id=" + edited.AccountID);
				} catch (ArgumentException ex) {
					ModelState.AddModelError ("Description", ex.Message);
				}
			}
			return PrepareEditTransactionView (transaction);
		}

		public ActionResult DeleteTransaction (int? TransactionID)
		{
			Transaction transaction = context.Transactions.Find (TransactionID);
			if (transaction == null)
				return ErrorResult ("Transaction not found", 404);
			int accid = transaction.AccountID;
			ActionResult r = CheckForAccountPermissions (accid);
			if (r != null)
				return r;
			try {
				account.RemoveTransaction (transaction, context);
			} catch (ArgumentException ex) {
				return Redirect ("/account/details?id=" + accid + "&message=" + HttpUtility.UrlEncode (ex.Message));
			}
			return Redirect ("/account/details?id=" + accid);
		}

		private Account account;

		public ActionResult Details (string message, DateTime? dateFrom, DateTime? dateTo)
		{
			if (account == null)
				return RedirectToHome ();
			if (dateFrom != null || dateTo != null) {
				if (dateFrom == null)
					dateFrom = Utils.MinDateTime;
				if (dateTo == null)
					dateTo = DateTime.Now;
				long unixFrom = Utils.DateToUnix (dateFrom.Value);
				long unixTo = Utils.DateToUnix (dateTo.Value); 
				account.Transactions = context.Transactions
					.Where (t => t.AccountID == account.AccountID &&
				t.TransactionTimeValue >= unixFrom &&
				t.TransactionTimeValue <= unixTo).ToList ();
				ViewData ["dateFrom"] = dateFrom.Value.ToString (Strings.FORMAT_DATE);
				ViewData ["dateTo"] = dateTo.Value.ToString (Strings.FORMAT_DATE);
			}
			account.Transactions = account.Transactions.OrderBy (t => t.TransactionTimeValue).ToList ();
			ViewData ["message"] = message;
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
			return PrepareCreateTransactionView (transaction);
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
				.Include ("Transactions.Category")
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
