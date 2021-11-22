namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class PropDayOfWeek
	{
		[Fact]
		public static void AllOf2020()
		{
			DateTime dt = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			UtcDateTime udt = new(2020, 1, 1, 0, 0, 0);
			for (int i = 0; i < 365; i++)
			{
				DayOfWeek expected = dt.DayOfWeek;
				DayOfWeek actual = udt.DayOfWeek;
				Assert.Equal(expected, actual);
				dt = dt.AddDays(1);
				udt = udt.AddDays(1);
			}
		}
	}
}
