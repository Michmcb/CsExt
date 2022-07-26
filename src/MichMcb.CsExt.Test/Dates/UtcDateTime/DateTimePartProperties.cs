namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using MichMcb.CsExt.Rng;
	using System;
	using Xunit;

	public static class DateTimePartProperties
	{
		[Fact]
		public static void CtorAndDateParts()
		{
			UtcDateTime dt = new(2020, 7, 14, 16, 24, 59, 129);
			dt.Deconstruct(out int year, out int month, out int day, out int hour, out int min, out int sec, out int ms, out int remainder);
			Assert.Equal(2020, year);
			Assert.Equal(7, month);
			Assert.Equal(14, day);
			Assert.Equal(16, hour);
			Assert.Equal(24, min);
			Assert.Equal(59, sec);
			Assert.Equal(129, ms);
			Assert.Equal(0, remainder);
			Assert.Equal(2020, dt.Year);
			Assert.Equal(7, dt.Month);
			Assert.Equal(14, dt.Day);
			Assert.Equal(16, dt.Hour);
			Assert.Equal(24, dt.Minute);
			Assert.Equal(59, dt.Second);
			Assert.Equal(129, dt.Millisecond);
		}
		[Fact]
		public static void DatesSurviveRoundTrip()
		{
			LcgRng rng = new();
			for (int year = 1; year <= 9999; year++)
			{
				for (int month = 1; month <= 12; month++)
				{
					int daysInMonth = DateTime.DaysInMonth(year, month);
					for (int day = 1; day <= daysInMonth; day++)
					{
						// Takes too long to do all the hours/mins/secs/millis, so just use a random value for each one
						int hour = rng.NextInt32(0, 24);
						int minute = rng.NextInt32(0, 60);
						int second = rng.NextInt32(0, 60);
						int millis = rng.NextInt32(0, 1000);

						UtcDateTime dt = new(year, month, day, hour, minute, second, millis);
						dt.Deconstruct(out int y, out int mon, out int d, out int h, out int min, out int s, out int ms, out int remainder);
						Assert.Equal(year, y);
						Assert.Equal(month, mon);
						Assert.Equal(day, d);
						Assert.Equal(hour, h);
						Assert.Equal(minute, min);
						Assert.Equal(second, s);
						Assert.Equal(millis, ms);
						Assert.Equal(0, remainder);
					}
				}
			}
		}
	}
}
