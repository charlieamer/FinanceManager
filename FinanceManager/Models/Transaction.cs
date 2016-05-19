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
		public DateTime? TransactionTime {
			get {
				if (TransactionTimeValue == 0)
					return null;
				return Utils.UnixToDate (TransactionTimeValue);
			}
			set {
				if (value.HasValue)
					TransactionTimeValue = Utils.DateToUnix (value.Value);
				else
					TransactionTimeValue = 0;
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
		public int CategoryID {
			get;
			set;
		}

		public Category Category {
			get;
			set;
		}

		public float BalanceBefore {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public float BalanceAfter {
			get {
				if (Type == TransactionType.Deposit)
					return BalanceBefore + Amount;
				else if (Type == TransactionType.Withdraw)
					return BalanceBefore - Amount;
				else
					throw new NotImplementedException ();
			}
		}

		public void Delete (ModelContext modelContext)
		{
			modelContext.Transactions.Remove (this);
			modelContext.SaveChanges ();
		}

		public void FromAnother (Transaction transaction)
		{
			this.Amount = transaction.Amount;
			this.Description = transaction.Description;
			this.TransactionTime = transaction.TransactionTime;
			this.Type = transaction.Type;
			this.CategoryID = transaction.CategoryID;
		}
	}
}

