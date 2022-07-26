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
			for (int expectedHour = -13; expectedHour < 14; expectedHour++)
			{
				for (int expectedMinute = 0; expectedMinute < 59; expectedMinute++)
				{
					Check(expectedHour, expectedMinute);
				}
			}
			// -14:00 and 14:00 are in range, but -14:01 and 14:01 are not.
			Check(-14, 0);
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
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(10, 999));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(10, -999));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(15, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(-15, 0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(14, 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(14, -1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(Tz.MaxTicks + 1));
			Assert.Throws<ArgumentOutOfRangeException>(() => new Tz(Tz.MinTicks - 1));
		}
		private static void Check(int expectedHour, int expectedMinute)
		{
			// Negatives minutes should be the same as positive minutes

			Tz tzp = Tz.TryCreate(expectedHour, expectedMinute).ValueOrException();
			Tz tzn = Tz.TryCreate(expectedHour, -expectedMinute).ValueOrException();
			Assert.Equal(tzp, tzn);
			Assert.Equal(tzp.Ticks, tzn.Ticks);
			Assert.Equal(tzp.Hours, tzn.Hours);
			Assert.Equal(tzp.Minutes, tzn.Minutes);

			tzp.GetAbsoluteParts(out int actualHour, out int actualMinute, out bool p);
			Assert.Equal(expectedHour < 0 ? -expectedHour : expectedHour, actualHour);
			Assert.Equal(expectedMinute, actualMinute);
			Assert.Equal(expectedHour >= 0, p);
			Assert.Equal(expectedHour, tzp.Hours);
			if (p)
			{
				Assert.Equal(expectedMinute, tzp.Minutes);
			}
			else
			{
				Assert.Equal(-expectedMinute, tzp.Minutes);
			}
		}
	}
}
