namespace MichMcb.CsExt.Test.Dates.DateUtil
{
	using System;
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class WriteYearMonthDay
	{
		[Fact]
		public static void Good()
		{
			Assert.Equal("2010-12-01", DateUtil.YearMonthDayToString(2010, 12, 1, true));
			Assert.Equal("20101201", DateUtil.YearMonthDayToString(2010, 12, 1, false));
			Assert.Equal("2010-12-01", DateUtil.YearMonthDayToString(new DateOnly(2010, 12, 1), true));
			Assert.Equal("20101201", DateUtil.YearMonthDayToString(new DateOnly(2010, 12, 1), false));
			Assert.Equal("2010-12-01", DateUtil.YearMonthDayToString(new DateTime(2010, 12, 1, 1, 2, 3, DateTimeKind.Local), true));
			Assert.Equal("20101201", DateUtil.YearMonthDayToString(new DateTime(2010, 12, 1, 1, 2, 3, DateTimeKind.Utc), false));
		}
		[Fact]
		public static void Bad()
		{
			Span<char> undersized = stackalloc char[5];
			Assert.Equal(-10, DateUtil.WriteYearMonthDay(undersized, 2010, 12, 1, true));
			Assert.Equal(-8, DateUtil.WriteYearMonthDay(undersized, 2010, 12, 1, false));
		}
	}
}
