using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace FinanceManager.Controllers
{
	public class UserController : BaseController
	{
		protected override string[] AuthenticatedRoutes {
			get {
				return null;
			}
		}

		[HttpGet]
		public ActionResult Login ()
		{
			return View ();
			//int a = ViewData.ModelState ["123"];
		}

		[HttpPost]
		public ActionResult Login (string username, string password)
		{
			User found = context.Users.Where (user =>
				(user.Email == username || user.Username == username) && user.Password == password)
				.FirstOrDefault ();
			if (found == null) {
				ViewData ["message"] = "Username or password don't match";
				return View ();
			} else {
				SessionLogin (found);
				return RedirectToHome ();
			}
		}

		public string GetImagesRoot ()
		{
			return Server.MapPath (Strings.PATH_IMAGES);
		}

		Image SaveAvatar (HttpPostedFileBase file)
		{
			if (file.ContentLength == 0)
				return null;
			if (!HttpPostedFileBaseExtensions.IsImage (file)) {
				ModelState.AddModelError ("ImageFile", "Uploaded file is not an image");
				return null;
			}
			string extension = Path.GetExtension (file.FileName);
			string fname = Path.GetRandomFileName ();

			Image avatar = new Image ();
			avatar.Path = fname + extension;
			avatar.BasePath = GetImagesRoot ();
			try {
				avatar = context.Images.Add (avatar);
				context.SaveChanges ();
			} catch (Exception e) {
				ModelState.AddModelError ("ImageFile", e.Message);
				return null;
			}

			try {
				file.SaveAs (GetImagesRoot () + avatar.Path);
			} catch (Exception e) {
				avatar.Delete (context);
				ModelState.AddModelError ("ImageFile", e.Message);
				return null;
			}
			return avatar;
		}

		[HttpGet]
		public ActionResult DeleteProfile ()
		{
			User user = SessionUser;
			if (user != null && user.Image != null) {
				user.Image.Delete (context);
			}
			return RedirectToHome ();
		}

		[HttpPost]
		public ActionResult UploadProfile ()
		{
			DeleteProfile ();
			if (Request.Files.Count > 0 && IsLoggedIn ()) {
				Image img = SaveAvatar (Request.Files [0]);
				if (img != null) {
					SessionUser.Image = img;
					SessionUser.ImageID = img.ImageID;
					context.SaveChanges ();
				}
			}
			return RedirectToHome ();
		}

		[HttpGet]
		public ActionResult Register ()
		{
			return View ();
		}

		[HttpPost]
		public ActionResult Register (User user)
		{
			// Password repeat validation
			string Password2 = "";
			if (Request.Params ["Password2"] != null)
				Password2 = Request.Params ["Password2"];

			if (Password2 != user.Password)
				ModelState.AddModelError ("Password2", "Passwords don't match");

			// Image upload validation
			Image img = null;
			if (Request.Files.Count > 0)
				img = SaveAvatar (Request.Files [0]);

			if (img != null) {
				user.ImageID = img.ImageID;
				user.Image = img;
			}

			// Username duplicate validation
			User test = context.Users
				.Where (u => u.Username.ToLower () == user.Username.ToLower ())
				.FirstOrDefault ();
			if (test != null)
				ModelState.AddModelError ("Username", "Username is taken");

			// Email duplicate validation
			test = context.Users
				.Where (u => u.Email.ToLower () == user.Email.ToLower ())
				.FirstOrDefault ();
			if (test != null)
				ModelState.AddModelError ("Email", "Email is taken");

			// Final model validation
			if (ModelState.IsValid) {
				user.Create (context);
				return Redirect ("/user/login");
			} else {
				if (img != null)
					img.Delete (context);
			}

			context.SaveChanges ();
			return View (user);
		}

		[HttpGet]
		public ActionResult Logout ()
		{
			SessionLogout ();
			return RedirectToHome ();
		}
	}
}