namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class PropDayOfWeek
	{
		[Fact]
		public static void Year2000ToYear3000()
		{
			UtcDateTime max = new(3000, 1, 1, 0, 0, 0);
			DateTime dt = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			UtcDateTime udt = new(2000, 1, 1, 0, 0, 0);
			while (udt != max)
			{
				{
					DayOfWeek expected = dt.DayOfWeek;
					DayOfWeek actual = udt.IsoDayOfWeek.DayOfWeek();
					Assert.Equal(expected, actual);
				}
				{
					IsoDayOfWeek expected = udt.DayOfWeek.IsoDayOfWeek();
					IsoDayOfWeek actual = udt.IsoDayOfWeek;
					Assert.Equal(expected, actual);
				}
				dt = dt.AddDays(1);
				udt = udt.AddDays(1);
			}
		}
	}
}
