namespace MichMcb.CsExt.Test.Dates.DateTimeExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public sealed class Deconstruction
	{
		[Fact]
		public static void GetDateTimeParts()
		{
			{
				new DateTime(2020, 7, 14, 16, 24, 59, 123).Deconstruct(out int year, out int month, out int day, out int hour, out int minute, out int second, out int millis, out int remainder);
				Assert.Equal(2020, year);
				Assert.Equal(7, month);
				Assert.Equal(14, day);
				Assert.Equal(16, hour);
				Assert.Equal(24, minute);
				Assert.Equal(59, second);
				Assert.Equal(123, millis);
				Assert.Equal(0, remainder);
			}
			{
				(int year, int month, int day, int hour, int minute, int second, int millis, int remainder) = new DateTime(2020, 7, 14, 16, 24, 59, 123);
				Assert.Equal(2020, year);
				Assert.Equal(7, month);
				Assert.Equal(14, day);
				Assert.Equal(16, hour);
				Assert.Equal(24, minute);
				Assert.Equal(59, second);
				Assert.Equal(123, millis);
				Assert.Equal(0, remainder);
			}
		}
		[Fact]
		public static void GetDateParts()
		{
			new DateTime(2020, 7, 14, 16, 24, 59, 123).GetDateParts(out int year, out int month, out int day);
			Assert.Equal(2020, year);
			Assert.Equal(7, month);
			Assert.Equal(14, day);
		}
		[Fact]
		public static void GetTimeParts()
		{
			new DateTime(2020, 7, 14, 16, 24, 59, 123).GetTimeParts(out int hour, out int minute, out int second, out int millis, out int remainder);
			Assert.Equal(16, hour);
			Assert.Equal(24, minute);
			Assert.Equal(59, second);
			Assert.Equal(123, millis);
			Assert.Equal(0, remainder);
		}
	}
}
