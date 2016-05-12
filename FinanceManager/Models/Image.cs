using System;

namespace FinanceManager
{
	public class Image
	{
		public int ImageID {
			get;
			set;
		}

		public String Path {
			get;
			set;
		}

		public void Delete (ModelContext context)
		{
			System.IO.File.Delete (Path);
			context.Images.Remove (this);
		}
	}

}

