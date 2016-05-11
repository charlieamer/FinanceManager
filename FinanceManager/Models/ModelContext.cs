using System;
using System.Data.Entity;

namespace FinanceManager
{
	public class ModelContext : DbContext
	{
		public DbSet<User> Users {
			get;
			set;
		}
	}
}

