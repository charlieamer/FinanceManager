using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinanceManager
{
	public class Image
	{
		[Key]
		public int ImageID {
			get;
			set;
		}

		[Required]
		public String Path {
			get;
			set;
		}

		public String BasePath {
			get;
			set;
		}

		public String RenderPath {
			get {
				return Strings.PATH_IMAGES + Path;
			}
		}

		public List<User> GetUsers (ModelContext context)
		{
			return context.Users
				.Include ("Image")
				.Where (usr => usr.ImageID == ImageID)
				.ToList ();
		}

		public void Delete (ModelContext context)
		{
			System.IO.File.Delete (BasePath + Path);
			context.Images.Attach (this);
			context.Images.Remove (this);
			foreach (User user in GetUsers(context)) {
				user.ImageID = null;
			}
			context.SaveChanges ();
		}
	}

}

