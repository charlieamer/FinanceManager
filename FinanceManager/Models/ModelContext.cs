using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity.ModelConfiguration.Conventions;

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
			System.Console.WriteLine ("Seeding database ...");
			ModelContext context = this;
			Clear ();
			User user = new User () {
				Username = "hepek",
				Email = "hepek@hepek.com",
				Password = "hepek",
				Admin = true
			};
			user.Create (context);

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
				TransactionTime = DateTime.Today,
				CategoryID = user.GetCategoryByName ("Other").CategoryID
			};
			account.AddTransaction (t1, context);

			Transaction t2 = new Transaction () {
				Description = "First deposit",
				Amount = 200,
				Type = TransactionType.Deposit,
				TransactionTime = DateTime.Today,
				CategoryID = user.GetCategoryByName ("Other").CategoryID
			};
			account.AddTransaction (t2, context);

			// This is functional test. This should throw error
			Transaction t3 = new Transaction () {
				Description = "Error transaction",
				Amount = 2000,
				Type = TransactionType.Withdraw,
				TransactionTime = DateTime.Today.AddDays (-0.5),
				CategoryID = user.GetCategoryByName ("Other").CategoryID
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
				TransactionTime = DateTime.Today.AddDays (-1),
				CategoryID = user.GetCategoryByName ("Other").CategoryID
			};
			account.AddTransaction (t4, context);
			context.SaveChanges ();

			System.Console.WriteLine ("Database seeded successfuly");
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

		protected override void OnModelCreating (DbModelBuilder modelBuilder)
		{
			base.OnModelCreating (modelBuilder);
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention> ();
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

		public DbSet<Category> Categories {
			get;
			set;
		}
	}
}

