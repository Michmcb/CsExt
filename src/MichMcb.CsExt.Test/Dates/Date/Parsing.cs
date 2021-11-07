namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class Parsing
	{
		[Fact]
		public static void Good()
		{
			Assert.True(Date.TryParseIso8601String("2020-10-20", true).Success(out Date d));
			Assert.Equal(new Date(2020, 10, 20), d);

			Assert.True(Date.TryParseIso8601String("20201020", true).Success(out d));
			Assert.Equal(new Date(2020, 10, 20), d);

			Assert.True(Date.TryParseIso8601String("2020-100", true).Success(out d));
			Assert.Equal(new Date(2020, 4, 9), d);

			Assert.True(Date.TryParseIso8601String("2020100", true).Success(out d));
			Assert.Equal(new Date(2020, 4, 9), d);

			Assert.True(Date.TryParseIso8601String("2020-10-20T10:00:50", false).Success(out d));
			Assert.Equal(new Date(2020, 10, 20), d);
		}
		[Fact]
		public static void Bad()
		{
			Assert.Equal("Years part was not 4 digits long, it was: 0", Date.TryParseIso8601String("", true).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 1", Date.TryParseIso8601String("2", true).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 2", Date.TryParseIso8601String("20", true).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 3", Date.TryParseIso8601String("202", true).ErrorOr(null));

			// Only year is not enough
			Assert.Equal("String only consists of a Year part", Date.TryParseIso8601String("2020", true).ErrorOr(null));
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 0", Date.TryParseIso8601String("2020-", true).ErrorOr(null));

			// 5 digits, not good
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 1", Date.TryParseIso8601String("20201", true).ErrorOr(null));
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 1", Date.TryParseIso8601String("2020-1", true).ErrorOr(null));

			Assert.Equal("Parsed only a year and month without a separator, which ISO-8601 disallows because it can be confused with yyMMdd. Only yyyy-MM is valid, not yyyyMM", Date.TryParseIso8601String("202011", true).ErrorOr(null));

			Assert.Equal("Ordinal Day is not all latin digits: X67", Date.TryParseIso8601String("2020-X67", true).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 1X7", Date.TryParseIso8601String("2020-1X7", true).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 16X", Date.TryParseIso8601String("2020-16X", true).ErrorOr(null));

			Assert.Equal("Was expecting only the date component of an ISO-8601, but it has time: 2020-10-20T10:00:50", Date.TryParseIso8601String("2020-10-20T10:00:50", true).ErrorOr(null));
		}
	}
}
