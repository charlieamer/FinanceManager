using System;
using System.Data.Entity;

namespace FinanceManager
{
	public class ModelContext : DbContext
	{
		public ModelContext () : base ()
		{
			Database.SetInitializer<ModelContext> (new DropCreateDatabaseIfModelChanges<ModelContext> ());
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

