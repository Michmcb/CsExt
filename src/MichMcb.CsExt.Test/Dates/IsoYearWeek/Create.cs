namespace MichMcb.CsExt.Test.Dates.IsoYearWeek
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Create
	{
		[Fact]
		public static void BadInputs()
		{
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(-50, 1, 1).ValueOrException());
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(99999, 1, 1).ValueOrException());
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(2001, -20, 1).ValueOrException());
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(2001, 300, 1).ValueOrException());
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(2001, 1, -50).ValueOrException());
			Assert.Throws<NoValueException>(() => IsoYearWeek.TryCreate(2001, 1, 1000).ValueOrException());
		}
		[Fact]
		public static void Success()
		{
			// Wikipedia provided these dates
			// Monday 29 December 2008 is written "2009-W01-1"
			// Sunday 3 January 2010 is written "2009-W53-7"

			IsoYearWeek dec29th2008 = new(2009, 1, IsoDayOfWeek.Monday);
			IsoYearWeek jan1st2009 = new(2009, 1, IsoDayOfWeek.Thursday);
			IsoYearWeek jan3rd2010 = new(2009, 53, IsoDayOfWeek.Sunday);

			Test(dec29th2008,2008, 12, 29);
			Test(jan1st2009, 2009, 1, 1);
			Test(jan3rd2010, 2010, 1, 3);
		}
		private static void Test(IsoYearWeek expected, int year, int month, int day)
		{
			DateOnly ymd = new(year, month, day);
			AreEqual(expected, IsoYearWeek.TryCreate(year, month, day).ValueOrException());
			AreEqual(expected, IsoYearWeek.Create(ymd));
			AreEqual(expected, IsoYearWeek.Create(ymd.ToDateTime(default, DateTimeKind.Utc)));
			AreEqual(expected, IsoYearWeek.Create(UtcDateTime.FromDateOnly(ymd, default)));
		}
		private static void AreEqual(IsoYearWeek expected, IsoYearWeek actual)
		{
			Assert.Equal(expected.Year, actual.Year);
			Assert.Equal(expected.Week, actual.Week);
			Assert.Equal(expected.WeekDay, actual.WeekDay);
		}
	}
}
