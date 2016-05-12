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
		[HttpGet]
		public ActionResult Login ()
		{
			return View ();
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
				return RedirectToAction ("Index", "Home");
			}
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
			string fname = Server.MapPath ("~/Images/") + Path.GetRandomFileName ();

			Image avatar = new Image ();
			avatar.Path = fname + extension;
			try {
				avatar = context.Images.Add (avatar);
				context.SaveChanges ();
			} catch (Exception e) {
				ModelState.AddModelError ("ImageFile", e.Message);
				return null;
			}

			try {
				file.SaveAs (avatar.Path);
			} catch (Exception e) {
				avatar.Delete (context);
				ModelState.AddModelError ("ImageFile", e.Message);
				return null;
			}
			return avatar;
		}

		[HttpGet]
		public ActionResult Register ()
		{
			return View ();
		}

		[HttpPost]
		public ActionResult Register (User user)
		{
			string Password2 = "";
			if (Request.Params ["Password2"] != null)
				Password2 = Request.Params ["Password2"];

			if (Password2 != user.Password)
				ModelState.AddModelError ("Password2", "Passwords don't match");

			Image img = null;
			if (Request.Files.Count > 0)
				img = SaveAvatar (Request.Files [0]);

			if (img != null) {
				user.ImageID = img.ImageID;
				user.Image = img;
			}

			if (ModelState.IsValid) {
				context.Users.Add (user);
				context.SaveChanges ();
				return RedirectToAction ("Index", "Home");
			} else {
				if (img != null)
					img.Delete (context);
			}

			context.SaveChanges ();
			return View (user);
		}
	}
}