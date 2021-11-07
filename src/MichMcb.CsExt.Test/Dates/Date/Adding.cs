namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Adding
	{
		[Fact]
		public static void AddYears()
		{
			Date dt = new(2004, 2, 29);
			Date result = dt.AddYears(1);
			Assert.Equal(2005, result.Year);
			Assert.Equal(2, result.Month);
			Assert.Equal(28, result.Day);

			dt = new Date(1, 1, 31);
			for (int i = 1; i < 9998; i++)
			{
				result = dt.AddYears(i);
				result.Deconstruct(out int y, out int m, out int d);
				Assert.Equal(i + 1, y);
				Assert.Equal(1, m);
				Assert.Equal(DateTime.DaysInMonth(i + 1, 1), d);
			}
		}
		[Fact]
		public static void AddPositiveMonths()
		{
			Date dt = new(2000, 1, 31);
			Date result = dt.AddMonths(0);
			Assert.Equal(dt, result);

			result = dt.AddMonths(1);
			Assert.Equal(new Date(2000, 2, 29), result);

			result = dt.AddMonths(2);
			Assert.Equal(new Date(2000, 3, 31), result);

			result = dt.AddMonths(3);
			Assert.Equal(new Date(2000, 4, 30), result);

			result = dt.AddMonths(4);
			Assert.Equal(new Date(2000, 5, 31), result);

			result = dt.AddMonths(5);
			Assert.Equal(new Date(2000, 6, 30), result);

			result = dt.AddMonths(6);
			Assert.Equal(new Date(2000, 7, 31), result);

			result = dt.AddMonths(7);
			Assert.Equal(new Date(2000, 8, 31), result);

			result = dt.AddMonths(8);
			Assert.Equal(new Date(2000, 9, 30), result);

			result = dt.AddMonths(9);
			Assert.Equal(new Date(2000, 10, 31), result);

			result = dt.AddMonths(10);
			Assert.Equal(new Date(2000, 11, 30), result);

			result = dt.AddMonths(11);
			Assert.Equal(new Date(2000, 12, 31), result);

			result = dt.AddMonths(12);
			Assert.Equal(new Date(2001, 1, 31), result);

			result = dt.AddMonths(13);
			Assert.Equal(new Date(2001, 2, 28), result);

			result = dt.AddMonths(14);
			Assert.Equal(new Date(2001, 3, 31), result);

			result = dt.AddMonths(15);
			Assert.Equal(new Date(2001, 4, 30), result);

			result = dt.AddMonths(16);
			Assert.Equal(new Date(2001, 5, 31), result);

			result = dt.AddMonths(17);
			Assert.Equal(new Date(2001, 6, 30), result);

			result = dt.AddMonths(18);
			Assert.Equal(new Date(2001, 7, 31), result);

			result = dt.AddMonths(19);
			Assert.Equal(new Date(2001, 8, 31), result);

			result = dt.AddMonths(20);
			Assert.Equal(new Date(2001, 9, 30), result);

			result = dt.AddMonths(21);
			Assert.Equal(new Date(2001, 10, 31), result);

			result = dt.AddMonths(22);
			Assert.Equal(new Date(2001, 11, 30), result);

			result = dt.AddMonths(23);
			Assert.Equal(new Date(2001, 12, 31), result);

			result = dt.AddMonths(24);
			Assert.Equal(new Date(2002, 1, 31), result);
		}
		[Fact]
		public static void AddNegativeMonths()
		{
			Date dt = new(2000, 1, 31);
			Date result = dt.AddMonths(0);
			Assert.Equal(dt, result);

			result = dt.AddMonths(-1);
			Assert.Equal(new Date(1999, 12, 31), result);

			result = dt.AddMonths(-2);
			Assert.Equal(new Date(1999, 11, 30), result);

			result = dt.AddMonths(-3);
			Assert.Equal(new Date(1999, 10, 31), result);

			result = dt.AddMonths(-4);
			Assert.Equal(new Date(1999, 9, 30), result);

			result = dt.AddMonths(-5);
			Assert.Equal(new Date(1999, 8, 31), result);

			result = dt.AddMonths(-6);
			Assert.Equal(new Date(1999, 7, 31), result);

			result = dt.AddMonths(-7);
			Assert.Equal(new Date(1999, 6, 30), result);

			result = dt.AddMonths(-8);
			Assert.Equal(new Date(1999, 5, 31), result);

			result = dt.AddMonths(-9);
			Assert.Equal(new Date(1999, 4, 30), result);

			result = dt.AddMonths(-10);
			Assert.Equal(new Date(1999, 3, 31), result);

			result = dt.AddMonths(-11);
			Assert.Equal(new Date(1999, 2, 28), result);

			result = dt.AddMonths(-12);
			Assert.Equal(new Date(1999, 1, 31), result);

			result = dt.AddMonths(-13);
			Assert.Equal(new Date(1998, 12, 31), result);

			result = dt.AddMonths(-14);
			Assert.Equal(new Date(1998, 11, 30), result);

			result = dt.AddMonths(-15);
			Assert.Equal(new Date(1998, 10, 31), result);

			result = dt.AddMonths(-16);
			Assert.Equal(new Date(1998, 9, 30), result);

			result = dt.AddMonths(-17);
			Assert.Equal(new Date(1998, 8, 31), result);

			result = dt.AddMonths(-18);
			Assert.Equal(new Date(1998, 7, 31), result);

			result = dt.AddMonths(-19);
			Assert.Equal(new Date(1998, 6, 30), result);

			result = dt.AddMonths(-20);
			Assert.Equal(new Date(1998, 5, 31), result);

			result = dt.AddMonths(-21);
			Assert.Equal(new Date(1998, 4, 30), result);

			result = dt.AddMonths(-22);
			Assert.Equal(new Date(1998, 3, 31), result);

			result = dt.AddMonths(-23);
			Assert.Equal(new Date(1998, 2, 28), result);

			result = dt.AddMonths(-24);
			Assert.Equal(new Date(1998, 1, 31), result);

			result = dt.AddMonths(-25);
			Assert.Equal(new Date(1997, 12, 31), result);
		}
		[Fact]
		public static void AddDays()
		{
			Date dt = new(1999, 1, 1);
			Assert.Equal(1, dt.DayOfYear);
			for (int i = 1; i < 365; i++)
			{
				Assert.Equal(i + 1, dt.AddDays(i).DayOfYear);
			}

			// Leap year, should go up to 366
			dt = new Date(2000, 1, 1);
			Assert.Equal(1, dt.DayOfYear);
			for (int i = 1; i < 366; i++)
			{
				Assert.Equal(i + 1, dt.AddDays(i).DayOfYear);
			}
		}
	}
}
