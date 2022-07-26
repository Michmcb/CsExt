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
			Assert.Equal(12345, new UtcDateTime(12345).Ticks);
			Assert.Equal(0, new UtcDateTime(0).Ticks);
			Assert.Equal(DotNetTime.MaxTicks, new UtcDateTime(DotNetTime.MaxTicks).Ticks);
			Assert.Throws<ArgumentOutOfRangeException>(() => new UtcDateTime(DotNetTime.MaxTicks + 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new UtcDateTime(-1));
		}
		[Fact]
		public static void YearMonthDayHourMinuteSecondMillis()
		{
			Assert.Equal(0, new UtcDateTime(1, 1, 1, 0, 0, 0, 0).Ticks);
			Assert.Equal(DotNetTime.UnixEpochTicks, new UtcDateTime(1970, 1, 1, 0, 0, 0, 0).Ticks);
			Assert.Equal(DotNetTime.MaxTicks, UtcDateTime.MaxValue.Ticks);
			// Micros and 100-nanosecond increments
			Assert.Equal(9999, DotNetTime.MaxTicks - new UtcDateTime(9999, 12, 31, 23, 59, 59, 999).Ticks);
		}
	}
}
