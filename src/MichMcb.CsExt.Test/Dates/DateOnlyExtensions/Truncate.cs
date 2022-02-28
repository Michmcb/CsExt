namespace MichMcb.CsExt.Test.Dates.DateOnlyExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Truncate
	{
		[Fact]
		public static void TruncateToYear()
		{
			Assert.Equal(new DateOnly(2000, 1, 1), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Year));
		}
		[Fact]
		public static void TruncateToMonth()
		{
			Assert.Equal(new DateOnly(2000, 5, 1), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Month));
		}
		[Fact]
		public static void TruncateToDay()
		{
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Day));
		}
		[Fact]
		public static void TruncateToHour()
		{
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Hour));
		}
		[Fact]
		public static void TruncateToMinute()
		{
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Minute));
		}
		[Fact]
		public static void TruncateToSecond()
		{
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Second));
		}
		[Fact]
		public static void TruncateToMillisecond()
		{
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Millisecond));
		}
	}
}
