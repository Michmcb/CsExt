namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Ctor
	{
		[Fact]
		public static void Millis()
		{
			Assert.Equal(12345, new UtcDateTime(12345).TotalMilliseconds);
			Assert.Equal(0, new UtcDateTime(0).TotalMilliseconds);
			Assert.Equal(UtcDateTime.MaxMillis, new UtcDateTime(UtcDateTime.MaxMillis).TotalMilliseconds);
			Assert.Throws<ArgumentOutOfRangeException>(() => new UtcDateTime(UtcDateTime.MaxMillis + 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new UtcDateTime(-1));
		}
		[Fact]
		public static void YearMonthDayHourMinuteSecondMillis()
		{
			Assert.Equal(0, new UtcDateTime(1, 1, 1, 0, 0, 0, 0).TotalMilliseconds);
			Assert.Equal(UtcDateTime.UnixEpochMillis, new UtcDateTime(1970, 1, 1, 0, 0, 0, 0).TotalMilliseconds);
			Assert.Equal(UtcDateTime.MaxMillis, new UtcDateTime(9999, 12, 31, 23, 59, 59, 999).TotalMilliseconds);
		}
	}
}
