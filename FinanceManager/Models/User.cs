using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FinanceManager.Controllers;
using System.Collections.Generic;

namespace FinanceManager
{
	public class User
	{
		[Key]
		public int UserID {
			get;
			set;
		}

		[Required]
		[Index (IsUnique = true)]
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

		public void Invalidate ()
		{
			BaseController.InvalidateUser (this);
		}
	}
}

