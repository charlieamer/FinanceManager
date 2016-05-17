using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
		public long TransactionTimeValue {
			get;
			set;
		}

		[NotMapped]
		public DateTime TransactionTime {
			get {
				return DateTime.FromBinary (TransactionTimeValue);
			}
			set {
				TransactionTimeValue = value.ToBinary ();
			}
		}

		[Required]
		[Range (0, float.MaxValue)]
		public float Amount {
			get;
			set;
		}

		[Required]
		public TransactionType Type {
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

		public void Delete (ModelContext modelContext)
		{
			modelContext.Transactions.Remove (this);
			modelContext.SaveChanges ();
		}
	}
}

