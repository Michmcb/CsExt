namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;
	public static class CtorAndProperties
	{
		[Fact]
		public static void DayOfWeekAllOf2020()
		{
			DateTime dt = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			Date udt = new(2020, 1, 1);
			for (int i = 0; i < 365; i++)
			{
				DayOfWeek expected = dt.DayOfWeek;
				DayOfWeek actual = udt.DayOfWeek;
				Assert.Equal(expected, actual);
				dt = dt.AddDays(1);
				udt = udt.AddDays(1);
			}
		}
		[Fact]
		public static void DatesAreCorrect()
		{
			for (int year = 1; year <= 9999; year++)
			{
				int dayOfYear = 0;
				for (int month = 1; month <= 12; month++)
				{
					int daysInMonth = DateTime.DaysInMonth(year, month);
					for (int day = 1; day <= daysInMonth; day++)
					{
						++dayOfYear;
						Date dt = new(year, month, day);
						dt.Deconstruct(out int y, out int mon, out int d);
						Assert.Equal(year, y);
						Assert.Equal(month, mon);
						Assert.Equal(day, d);
						Assert.Equal(dayOfYear, dt.DayOfYear);
					}
				}
			}
		}
		[Fact]
		public static void Is29thOfFeb()
		{
			Assert.False(new Date(2020, 2, 28).Is29thFeb);
			Assert.True(new Date(2020, 2, 29).Is29thFeb);
			Assert.False(new Date(2020, 3, 1).Is29thFeb);

			Assert.False(new Date(2021, 2, 28).Is29thFeb);
			Assert.False(new Date(2021, 3, 1).Is29thFeb);
		}
		[Fact]
		public static void CtorAndDateParts()
		{
			Date dt = new(2020, 7, 14);
			dt.Deconstruct(out int year, out int month, out int day);
			Assert.Equal(2020, year);
			Assert.Equal(7, month);
			Assert.Equal(14, day);
			Assert.Equal(2020, dt.Year);
			Assert.Equal(7, dt.Month);
			Assert.Equal(14, dt.Day);
		}
	}
}
