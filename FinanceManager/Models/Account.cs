using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

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

		public string Description {
			get;
			set;
		}

		public int UserID {
			get;
			set;
		}

		public User User {
			get;
			set;
		}

		[Required]
		[Range (0, float.MaxValue, ErrorMessage = "Balance cannot be negative")]
		public float Balance {
			get;
			set;
		}

		IEnumerable<Transaction> Transactions;
	}
}

