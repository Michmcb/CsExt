namespace MichMcb.CsExt.Test.Dates.Tz
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class CtorAndProperties
	{
		[Fact]
		public static void TryCreate()
		{
			for (int expectedHour = -11; expectedHour < 14; expectedHour++)
			{
				for (int expectedMinute = 0; expectedMinute < 59; expectedMinute++)
				{
					Check(expectedHour, expectedMinute);
				}
			}
			// -12:00 and 14:00 are in range, but -12:01 and 14:01 are not.
			Check(-12, 0);
			Check(14, 0);
		}
		[Fact]
		public static void Edges()
		{
			Assert.Equal(Tz.MaxValue, new Tz(Tz.MaxTicks));
			Assert.Equal(Tz.MinValue, new Tz(Tz.MinTicks));
		}
		[Fact]
		public static void OutOfRange()
		{
			Assert.Equal("Timezone was out of range. Hours: -12 Minutes: 1", Tz.TryCreate(-12, 1).ErrorOr(null));
			Assert.Equal("Timezone was out of range. Hours: 14 Minutes: 1", Tz.TryCreate(14, 1).ErrorOr(null));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(Tz.MaxTicks + 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(Tz.MinTicks - 1));
		}
		private static void Check(int expectedHour, int expectedMinute)
		{
			Tz tz = Tz.TryCreate(expectedHour, expectedMinute).ValueOrException();
			tz.GetAbsoluteParts(out int actualHour, out int actualMinute, out bool p);
			Assert.Equal(expectedHour < 0 ? -expectedHour : expectedHour, actualHour);
			Assert.Equal(expectedMinute, actualMinute);
			Assert.Equal(expectedHour >= 0, p);
			Assert.Equal(expectedHour, tz.Hours);
			if (p)
			{
				Assert.Equal(expectedMinute, tz.Minutes);
			}
			else
			{
				Assert.Equal(-expectedMinute, tz.Minutes);
			}
		}
	}
}
