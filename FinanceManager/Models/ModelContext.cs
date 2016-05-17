using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;

namespace FinanceManager
{
	public class ModelContext : DbContext
	{
		public void Clear ()
		{
			List<User> users = Users.ToList ();
			foreach (User user in users) {
				user.Delete (this);
			}
			SaveChanges ();
		}

		public void Seed ()
		{
			ModelContext context = this;
			Clear ();
			User user = new User () {
				Username = "hepek",
				Email = "hepek@hepek.com",
				Password = "hepek",
				Admin = true
			};
			user = context.Users.Add (user);
			context.SaveChanges ();

			Account account = new Account () {
				UserID = user.UserID,
				Name = "Account with transactions",
				Description = "This is a test account which contains transactions",
				Balance = 1000
			};
			account = context.Accounts.Add (account);
			context.SaveChanges ();

			Transaction t1 = new Transaction () {
				Description = "First withdraw",
				Amount = 100,
				Type = TransactionType.Withdraw,
				TransactionTime = DateTime.Today
			};
			account.AddTransaction (t1, context);

			Transaction t2 = new Transaction () {
				Description = "First deposit",
				Amount = 200,
				Type = TransactionType.Deposit,
				TransactionTime = DateTime.Today
			};
			account.AddTransaction (t2, context);

			// This is functional test. This should throw error
			Transaction t3 = new Transaction () {
				Description = "Error transaction",
				Amount = 2000,
				Type = TransactionType.Withdraw,
				TransactionTime = DateTime.Today
			};
			bool thrown = false;
			try {
				account.AddTransaction (t3, context);
			} catch (ArgumentException) {
				thrown = true;
			}
			if (!thrown) {
				throw new Exception (
					"No exception occured for functional test (it should)"
				);
			}

			Transaction t4 = new Transaction () {
				Description = "First transaction",
				Amount = 200,
				Type = TransactionType.Deposit,
				TransactionTime = DateTime.Today.AddDays (-1)
			};
			account.AddTransaction (t4, context);
			List<Transaction> transactions = context.Transactions
				.Where (t => t.AccountID == account.AccountID)
				.ToList ()
				.OrderBy (t => t.TransactionTime)
				.ToList ();

			System.Console.WriteLine ("Database seeded successfuly");

			foreach (Transaction t in transactions) {
				System.Console.WriteLine (t.BalanceBefore);
			}
		}

		private class DbInitializer : DropCreateDatabaseIfModelChanges<ModelContext>
		{
			protected override void Seed (ModelContext context)
			{
				context.Seed ();
			}
		}

		public ModelContext () : base ()
		{
			Database.SetInitializer<ModelContext> (new DbInitializer ());
		}

		public DbSet<User> Users {
			get;
			set;
		}

		public DbSet<Image> Images {
			get;
			set;
		}

		public DbSet<Transaction> Transactions {
			get;
			set;
		}

		public DbSet<Account> Accounts {
			get;
			set;
		}
	}
}

