using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

		public int ImageID {
			get;
			set;
		}

		public Image Image {
			get;
			set;
		}
	}
}

