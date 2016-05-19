using System;

namespace FinanceManager
{
	public class Utils
	{
		public static long DateToUnix (DateTime datetime)
		{
			DateTime sTime = MinDateTime;
			return (long)(datetime - sTime).TotalSeconds;
		}

		public static DateTime UnixToDate (long unixtime)
		{
			DateTime sTime = MinDateTime;
			return sTime.AddSeconds (unixtime);

		}

		public static DateTime MinDateTime = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}

