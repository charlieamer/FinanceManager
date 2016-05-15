using System;
using System.ComponentModel.DataAnnotations;

namespace FinanceManager
{
	public class Transaction
	{
		[Key]
		public int TransactionID {
			get;
			set;
		}

		[Required]
		public Account Account {
			get;
			set;
		}

		[Required]
		public int AccountID {
			get;
			set;
		}

		[Required]
		public DateTime TransactionTime {
			get;
			set;
		}

		[Required]
		[Range (0, float.MaxValue)]
		public float Amount {
			get;
			set;
		}

		[Required]
		public bool IsWithdraw {
			get;
			set;
		}

		[Required]
		public float BalanceBefore {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}
	}
}

