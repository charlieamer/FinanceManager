using System;
using FinanceManager.Controllers;
using System.Web.Mvc;
using System.Linq;
using System.Web;

namespace FinanceManager
{
	public class CategoryController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return new string[] {
					"/category",
					"/category/index",
					"/category/create",
					"/category/delete",
					"/category/edit"
				};
			}
		}

		public ActionResult Index (string message)
		{
			ViewData ["message"] = message;
			return View ();
		}

		private ActionResult PrepareCreateView (Category category)
		{
			if (category == null) {
				category = new Category ();
				Random random = new Random ();
				category.Color = Strings.colors [random.Next (Strings.colors.Length)];
			}
			return View ("create", category);
		}

		public ActionResult Create ()
		{
			return PrepareCreateView (null);
		}

		[HttpPost]
		public ActionResult Create (Category category)
		{
			category.UserID = GetSessionUserID ();
			if (ModelState.IsValid) {
				context.Categories.Add (category);
				context.SaveChanges ();
				return RedirectToAction ("Index");
			}
			return PrepareCreateView (category);
		}

		Category category;

		private ActionResult CheckForPermissions (int? id)
		{
			category = context.Categories.Include ("Transactions")
				.Where (c => c.CategoryID == id).FirstOrDefault ();
			if (category == null)
				return ErrorResult ("Category not found", 404);
			if (category.UserID != GetSessionUserID ())
				return ErrorResult ("You don't own this category", 403);
			return null;
		}

		public ActionResult Edit (int? id)
		{
			ActionResult r = CheckForPermissions (id);
			if (r != null)
				return r;
			ViewData ["id"] = id;
			return PrepareCreateView (category);
		}

		[HttpPost]
		public ActionResult Edit (Category edited)
		{
			ActionResult r = CheckForPermissions (edited.CategoryID);
			if (r != null)
				return r;
			if (ModelState.IsValid) {
				category.FromAnother (edited);
				context.SaveChanges ();
				return Redirect ("/category");
			}
			return PrepareCreateView (edited);
		}

		public ActionResult Delete (int? id)
		{
			ActionResult r = CheckForPermissions (id);
			if (r != null)
				return r;
			if (category.Transactions.Count > 0)
				return Redirect ("/category?message=" +
				HttpUtility.UrlEncode ("Cannot delete category with transactions. Delete transactions first."));
			context.Categories.Remove (category);
			context.SaveChanges ();
			return Redirect ("/category");
		}

		protected override void OnActionExecuting (ActionExecutingContext filterContext)
		{
			base.OnActionExecuting (filterContext);
			if (IsLoggedIn ())
				SessionUser.Categories = context.Categories
					.Include ("Transactions")
					.Where (c => c.UserID == SessionUser.UserID)
					.ToList ();
		}
	}
}

