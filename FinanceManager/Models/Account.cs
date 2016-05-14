using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceManager
{
	public class Account
	{
		[Key]
		public int AccountID {
			get;
			set;
		}

		[Required]
		public string Name {
			get;
			set;
		}

		[Required]
		[Range (0, float.MaxValue)]
		public float Balance {
			get;
			set;
		}
	}
}

