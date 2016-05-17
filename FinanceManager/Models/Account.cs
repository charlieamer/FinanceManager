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

		public float CurrentBalance {
			get;
			set;
		}

		public ICollection<Transaction> Transactions {
			get;
			set;
		}

		public void ApplyTransaction (Transaction transaction)
		{
			transaction.BalanceBefore = CurrentBalance;
			if (transaction.Type == TransactionType.Deposit) {
				CurrentBalance += transaction.Amount;
			} else if (transaction.Type == TransactionType.Withdraw) {
				CurrentBalance -= transaction.Amount;
				if (CurrentBalance < 0)
					throw new ArgumentException (
						"Transaction " + transaction.TransactionID +
						" will make account balance negative");
			}
		}

		public void ApplyTransactions (List<Transaction> transactions)
		{
			transactions = transactions.OrderBy (t => t.TransactionTime).ToList ();
			float originalCurrentBalance = CurrentBalance;
			CurrentBalance = Balance;
			foreach (Transaction t in transactions) {
				try {
					ApplyTransaction (t);
				} catch (ArgumentException ex) {
					CurrentBalance = originalCurrentBalance;
					throw ex;
				}
			}
		}

		public void AddTransaction (Transaction transaction, ModelContext context)
		{
			transaction.AccountID = this.AccountID;
			List<Transaction> all = context.Transactions
				.Where (t => t.AccountID == this.AccountID)
				.ToList ();
			all.Add (transaction);
			try {
				ApplyTransactions (all);
				context.Transactions.Add (transaction);
				context.SaveChanges ();
			} catch (Exception ex) {
				throw ex;
			}
		}

		public void Delete (ModelContext modelContext)
		{
			Account theAccount = modelContext.Accounts
				.Include ("Transactions")
				.Where (a => AccountID == a.AccountID)
				.FirstOrDefault ();
			if (theAccount != null) {
				foreach (Transaction t in theAccount.Transactions.ToList()) {
					t.Delete (modelContext);
				}
				modelContext.Accounts.Remove (theAccount);
				modelContext.SaveChanges ();
			}
		}
	}
}

