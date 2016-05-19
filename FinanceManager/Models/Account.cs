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
			CurrentBalance = transaction.BalanceAfter;

			if (CurrentBalance < 0)
				throw new ArgumentException (
					"Action will make tis account have negative balance");
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

		public List<Transaction> GetTransactions (ModelContext context)
		{
			return context.Transactions
				.Where (t => t.AccountID == this.AccountID)
				.ToList ();
		}

		public void AddTransaction (Transaction transaction, ModelContext context)
		{
			transaction.AccountID = this.AccountID;
			List<Transaction> all = GetTransactions (context);
			all.Add (transaction);
			ApplyTransactions (all);
			context.Transactions.Add (transaction);
			context.SaveChanges ();
		}

		public void EditTransaction (Transaction edited, ModelContext context)
		{
			List<Transaction> all = GetTransactions (context);
			bool found = false;
			foreach (var t in all) {
				if (t.TransactionID == edited.TransactionID) {
					t.FromAnother (edited);
					found = true;
				}
			}
			if (!found)
				throw new ArgumentException ("Edited transaction was not found - " + edited.TransactionID);
			ApplyTransactions (all);
			context.SaveChanges ();
		}

		public void RemoveTransaction (Transaction transaction, ModelContext context)
		{
			List<Transaction> all = GetTransactions (context);
			Transaction toRemove = null;
			foreach (var t in all) {
				if (t.TransactionID == transaction.TransactionID) {
					toRemove = t;
				}
			}
			if (toRemove == null)
				throw new ArgumentException ("Transaction to delete was not found - " + transaction.TransactionID);
			else
				all.Remove (toRemove);
			ApplyTransactions (all);
			context.Transactions.Remove (transaction);
			context.SaveChanges ();
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

		public void FromAnother (Account account)
		{
			this.Name = account.Name;
			this.Description = account.Description;
		}
	}
}

