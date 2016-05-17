using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceManager.Controllers;
using System.Collections.Generic;
using System.Linq;

namespace FinanceManager
{
	public class User
	{
		public User ()
		{
			Admin = false;
		}

		[Key]
		public int UserID {
			get;
			set;
		}

		[Required]
		[StringLength (30)]
		public string Username {
			get;
			set;
		}

		[Required]
		public string Password {
			get;
			set;
		}

		[Required]
		[EmailAddress]
		public string Email {
			get;
			set;
		}

		[Required]
		public bool Admin {
			get;
			set;
		}

		public int? ImageID {
			get;
			set;
		}

		public Image Image {
			get;
			set;
		}

		public ICollection<Account> Accounts {
			get;
			set;
		}

		public string ProfileRenderPath {
			get {
				if (Image != null)
					return Image.RenderPath;
				else
					return Strings.PATH_NO_IMAGE;
			}
		}

		public void Delete (ModelContext modelContext)
		{
			User theUser = modelContext.Users
				.Include ("Accounts")
				.Include ("Image")
				.Where (u => u.UserID == UserID)
				.FirstOrDefault ();
			if (theUser != null) {
				foreach (Account acc in theUser.Accounts.ToList()) {
					acc.Delete (modelContext);
				}
				if (theUser.Image != null)
					theUser.Image.Delete (modelContext);
				modelContext.Users.Remove (theUser);
				modelContext.SaveChanges ();
			}
		}
	}
}

