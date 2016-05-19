using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FinanceManager
{
	public class Category
	{
		[Required]
		public string Name {
			get;
			set;
		}

		[Key]
		public int CategoryID {
			get;
			set;
		}

		[Required]
		[RegularExpression ("^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")]
		public string Color {
			get;
			set;
		}

		[Required]
		public int UserID {
			get;
			set;
		}

		public User User {
			get;
			set;
		}

		public ICollection<Transaction> Transactions {
			get;
			set;
		}

		public void Delete (ModelContext context)
		{
			context.Categories.Remove (this);
			context.SaveChanges ();
		}

		public void FromAnother (Category category)
		{
			this.Color = category.Color;
			this.Name = category.Name;
		}
	}
}

