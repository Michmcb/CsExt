namespace MichMcb.CsExt.Dates.UtcDateTime.Test
{
	using MichMcb.CsExt.Dates;
	using MichMcb.CsExt.Rng;
	using System;
	using Xunit;
	public sealed class UtcDateTimeTest
	{
		[Fact]
		public void DayOfWeekAllOf2020()
		{
			DateTime dt = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			UtcDateTime udt = new(2020, 1, 1, 0, 0, 0);
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
		public void TimeOfDayTest()
		{
			Assert.Equal(new TimeSpan(10, 12, 15), new UtcDateTime(2021, 1, 5, 10, 12, 15).TimeOfDay);
		}
		[Fact]
		public void Is29thFebTest()
		{
			Assert.False(new UtcDateTime(2020, 2, 28).Is29thFeb);
			Assert.True(new UtcDateTime(2020, 2, 29).Is29thFeb);
			Assert.False(new UtcDateTime(2020, 3, 1).Is29thFeb);

			Assert.False(new UtcDateTime(2021, 2, 28).Is29thFeb);
			Assert.False(new UtcDateTime(2021, 3, 1).Is29thFeb);
		}
		[Fact]
		public void PublicConstAndStaticFieldsCorrect()
		{
			int daysPer4Years = 0;
			int daysPer100Years = 0;
			int daysPer400Years = 0;
			for (int i = 1; i <= 4; i++)
			{
				daysPer4Years += DateTime.IsLeapYear(i) ? 366 : 365;
			}
			for (int i = 1; i <= 100; i++)
			{
				daysPer100Years += DateTime.IsLeapYear(i) ? 366 : 365;
			}
			for (int i = 1; i <= 400; i++)
			{
				daysPer400Years += DateTime.IsLeapYear(i) ? 366 : 365;
			}
			Assert.Equal(DateUtil.DaysPer4Years, daysPer4Years);
			Assert.Equal(DateUtil.DaysPer100Years, daysPer100Years);
			Assert.Equal(DateUtil.DaysPer400Years, daysPer400Years);

			long epochDays = 0;
			for (int i = 1; i < 1970; i++)
			{
				epochDays += DateTime.IsLeapYear(i) ? 366 : 365;
			}

			Assert.Equal(DateUtil.UnixEpochMillis, UtcDateTime.UnixEpoch.TotalMilliseconds);
			UtcDateTime minValue = new();
			Assert.Equal(0, minValue.TotalMilliseconds);
			Assert.Equal(0, UtcDateTime.MinValue.TotalMilliseconds);
			Assert.Equal(DateUtil.MaxMillis, UtcDateTime.MaxValue.TotalMilliseconds);
		}
		[Fact]
		public void FromDotNetDateTimes()
		{
			UtcDateTime converted = (UtcDateTime)new DateTime(2001, 1, 1, 15, 10, 10, DateTimeKind.Utc);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 15, 10, 10), converted);

			// I live in +10:00, no DST ever applied (thank goodness, screw DST)
			converted = (UtcDateTime)new DateTime(2001, 1, 1, 15, 10, 10, DateTimeKind.Local);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 5, 10, 10), converted);

			converted = (UtcDateTime)new DateTimeOffset(2001, 1, 1, 15, 10, 10, TimeSpan.Zero);
			Assert.Equal(new UtcDateTime(2001, 1, 1, 15, 10, 10), converted);

			converted = (UtcDateTime)new DateTimeOffset(2001, 1, 1, 15, 10, 10, new TimeSpan(5, 0, 0));
			Assert.Equal(new UtcDateTime(2001, 1, 1, 10, 10, 10), converted);
		}
		[Fact]
		public void ParseIso8601Strings()
		{
			//UtcDateTime.TryParseIso8601String("2020-W12", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020W12", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020-W12-5", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020W125", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020-W12T10", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020W12T10", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020-W12-5T10", TimeSpan.Zero);
			//UtcDateTime.TryParseIso8601String("2020W125T10", TimeSpan.Zero);


			// TODO it would be nice if we included the string that we tried to parse in the error message.
			// Empty string of course not good
			Assert.Equal("Years part was not 4 digits long, it was: 0", UtcDateTime.TryParseIso8601String("", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 2", UtcDateTime.TryParseIso8601String("20", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Years part was not 4 digits long, it was: 3", UtcDateTime.TryParseIso8601String("202", TimeSpan.Zero).ErrorOr(null));

			// Only year is not enough
			Assert.Equal("String only consists of a Year part", UtcDateTime.TryParseIso8601String("2020", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-", TimeSpan.Zero).ErrorOr(null));

			// 5 digits, not good
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20201", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Months/Weeks/Ordinal Days part was not at least 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-1", TimeSpan.Zero).ErrorOr(null));

			// Not actually valid; yyyyMM can be confused with yyyy-MM, so it isn't allowed
			Assert.Equal("Parsed only a year and month without a separator, which is disallowed because it can be confused with yyMMdd. Only yyyy-MM is valid, not yyyyMM", UtcDateTime.TryParseIso8601String("202011", TimeSpan.Zero).ErrorOr(null));
			// This is valid; yyyy-MM is not ambiguous unlike yyyyMM
			Assert.Equal(new UtcDateTime(2020, 11, 1), UtcDateTime.TryParseIso8601String("2020-11", TimeSpan.Zero).ValueOrException());

			// Interpreted as Ordinal (yyyy-ddd)
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020167", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020-167", TimeSpan.Zero).ValueOrException());

			// Non-digits
			Assert.Equal("Ordinal Day is not all latin digits: X67", UtcDateTime.TryParseIso8601String("2020-X67", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 1X7", UtcDateTime.TryParseIso8601String("2020-1X7", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 16X", UtcDateTime.TryParseIso8601String("2020-16X", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: X67", UtcDateTime.TryParseIso8601String("2020-X67T23", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 1X7", UtcDateTime.TryParseIso8601String("2020-1X7T23:10", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Ordinal Day is not all latin digits: 16X", UtcDateTime.TryParseIso8601String("2020-16XT23:10:52", TimeSpan.Zero).ErrorOr(null));

			// Weeks aren't supported
			Assert.Equal("ISO-8601 weeks are not supported", UtcDateTime.TryParseIso8601String("2020W10", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("ISO-8601 weeks are not supported", UtcDateTime.TryParseIso8601String("2020-w10", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("ISO-8601 weeks are not supported", UtcDateTime.TryParseIso8601String("2020w101", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("ISO-8601 weeks are not supported", UtcDateTime.TryParseIso8601String("2020-W10-1", TimeSpan.Zero).ErrorOr(null));

			// We don't do yyyy-MM-, because that gets interpreted as ordinal days, because it's only 3 chars until T or the end of the string
			Assert.Equal("Ordinal Day is not all latin digits: 06-", UtcDateTime.TryParseIso8601String("2020-06-", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Days part was not at least 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-1", TimeSpan.Zero).ErrorOr(null));

			// Interpreted as yyyy-MM-dd 00:00:00
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("20200615", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020-06-15", TimeSpan.Zero).ValueOrException());

			// Not allowed when we require a timezone
			Assert.Equal("This ISO-8601 time was missing a timezone designator: 2020-06-15", UtcDateTime.TryParseIso8601String("2020-06-15").ErrorOr(null));

			// Non-digits
			Assert.Equal("Day is not all latin digits: X5", UtcDateTime.TryParseIso8601String("202006X5", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Day is not all latin digits: X5", UtcDateTime.TryParseIso8601String("2020-06-X5", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Day is not all latin digits: 1X", UtcDateTime.TryParseIso8601String("2020061X", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Day is not all latin digits: 1X", UtcDateTime.TryParseIso8601String("2020-06-1X", TimeSpan.Zero).ErrorOr(null));

			// Inconsistent separators
			Assert.Equal("Separator between Year/Month was present, but separator between Month/Day was missing", UtcDateTime.TryParseIso8601String("2020-0615", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Separator between Year/Month was missing, but separator between Month/Day was present", UtcDateTime.TryParseIso8601String("202006-15", TimeSpan.Zero).ErrorOr(null));

			// Can't have the timezone yet
			Assert.Equal("Date and Time separator T was expected at index 8", UtcDateTime.TryParseIso8601String("20200615Z").ErrorOr(null));
			Assert.Equal("Date and Time separator T was expected at index 10", UtcDateTime.TryParseIso8601String("2020-06-15Z").ErrorOr(null));

			// Can't end with T, and need at least 2 digits after that
			Assert.Equal("Hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("20200615T", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20200615t2", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-06-15t", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-15T2", TimeSpan.Zero).ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:00:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("20200615T23", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("20200615t23", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15t23", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23Z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23+10:00", TimeSpan.Zero).ValueOrException());

			// Need at least 2 digits for the minute
			Assert.Equal("Minutes part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-06-15T23:", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Minutes part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20200615t231", TimeSpan.Zero).ErrorOr(null));
			Assert.Equal("Minutes part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-15t23:1", TimeSpan.Zero).ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:mm:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("20200615T2310", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23:10", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23:10Z", TimeSpan.Zero).ValueOrException());

			// Need at least 2 digits for the second
			Assert.Equal("Seconds part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:").ErrorOr(null));
			Assert.Equal("Seconds part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20200615T23105").ErrorOr(null));
			Assert.Equal("Seconds part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-15t23:10:5").ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:mm:ss
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("20200615T231052", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("20200615T231052Z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("     2020-06-15t23:10:52    ", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52Z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52+10:00", TimeSpan.Zero).ValueOrException());

			// Inconsistent separators
			Assert.Equal("Separator between Hour/Minute was present, but separator between Minute/Second was missing", UtcDateTime.TryParseIso8601String("2020-06-15T23:1052").ErrorOr(null));
			Assert.Equal("Separator between Hour/Minute was missing, but separator between Minute/Second was present", UtcDateTime.TryParseIso8601String("2020-06-15T2310:52").ErrorOr(null));

			// Need at least 1 millisecond digit
			Assert.Equal("Milliseconds separator was found but no milliseconds were found", UtcDateTime.TryParseIso8601String("20200615T231052.").ErrorOr(null));
			Assert.Equal("Milliseconds separator was found but no milliseconds were found", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.").ErrorOr(null));

			// Milliseconnndddssss
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 1), UtcDateTime.TryParseIso8601String("20200615T231052.1", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 12), UtcDateTime.TryParseIso8601String("20200615T231052.12", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052,1234", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052,12345", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123456", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 1), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.1", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 12), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52,123", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52,1234", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12345", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123456", TimeSpan.Zero).ValueOrException());

			// Everything with the Z timezone
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123Z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123Z", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123z", TimeSpan.Zero).ValueOrException());

			// Everything with an implicit timezone of +10:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123", new TimeSpan(10, 0, 0)).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123", new TimeSpan(10, 0, 0)).ValueOrException());

			// Timezone offset of +10:00 and +10:30
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615t231052.123+10", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123+1030", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123+10:30", TimeSpan.Zero).ValueOrException());

			// Max and min offsets
			Assert.Equal(new UtcDateTime(2020, 6, 15, 9, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123+14:00", TimeSpan.Zero).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 16, 11, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123-12:00", TimeSpan.Zero).ValueOrException());

			// Timezone char that's wrong
			Assert.Equal("Timezone designator was not valid, it was: X", UtcDateTime.TryParseIso8601String("20200615T231052.123X").ErrorOr(null));
			Assert.Equal("Timezone designator was not valid, it was: X", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123X").ErrorOr(null));

			// Timezone hours wrong length
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("20200615T231052.123+").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("20200615T231052.123-").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20200615T231052.123+1").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 0", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123-").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+1").ErrorOr(null));

			// Timezone minutes wrong
			Assert.Equal("Timezone minutes part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("20200615T231052.123+103").ErrorOr(null));
			Assert.Equal("Timezone minutes part was not 2 digits long, it was: 1", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10:3").ErrorOr(null));
		}
		[Fact]
		public void UtcDateTimeToString()
		{
			UtcDateTime dt = new(2020, 6, 5, 3, 0, 52, 012);
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToString());

			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringExtendedUtc(millis: true));
			Assert.Equal("20200605T030052.012Z", dt.ToIso8601StringBasicUtc(millis: true));
			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601StringExtendedUtc(millis: false));
			Assert.Equal("20200605T030052Z", dt.ToIso8601StringBasicUtc(millis: false));

			Assert.Equal("2020-06-05T13:00:52.012+10:00", dt.ToIso8601StringWithTimeZone(extended: true, millis: true));
			Assert.Equal("20200605T130052.012+1000", dt.ToIso8601StringWithTimeZone(extended: false, millis: true));
			Assert.Equal("2020-06-05T13:00:52+10:00", dt.ToIso8601StringWithTimeZone(extended: true, millis: false));
			Assert.Equal("20200605T130052+1000", dt.ToIso8601StringWithTimeZone(extended: false, millis: false));

			Assert.Equal("2020-06-05T03:00:52.012+00:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_ExtendedFormat_FullTz));
			Assert.Equal("20200605T030052.012Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_BasicFormat_UtcTz));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_ExtendedFormat_UtcTz));
			Assert.Equal("2020-06-05", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_DateOnly));
			Assert.Equal("20200605", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_DateOnlyWithoutSeparators));
			Assert.Equal("2020-157", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_DateOrdinal));
			Assert.Equal("--06-05", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_VcfUnknownYear));

			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz));
			Assert.Equal("20200605T03:00:52Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Separator_Time));

			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_ExtendedFormat_UtcTz));
			Assert.Equal("2020-06-05T03", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03+00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03+00:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T03:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00+00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00+00:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T03:00:52", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00:52+00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00:52+00:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T03:00:52.012", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00:52.012+00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00:52.012+00:00", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T06:00:52.012+03:00", dt.ToIso8601String(new TimeSpan(3, 0, 0), Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));


			dt = new(2020, 6, 5, 12, 0, 0);
			Assert.Equal("2020-06-05", dt.ToIso8601String(TimeSpan.Zero, Iso8601Parts.Format_DateOnly));
			Assert.Equal("2020-06-04", dt.ToIso8601String(new TimeSpan(-13, 0, 0), Iso8601Parts.Format_DateOnly));
			Assert.Equal("2020-06-06", dt.ToIso8601String(new TimeSpan(13, 0, 0), Iso8601Parts.Format_DateOnly));
		}
		[Fact]
		public void CtorAndDateParts()
		{
			UtcDateTime dt = new(2020, 7, 14, 16, 24, 59, 129);
			dt.Deconstruct(out int year, out int month, out int day, out int hour, out int min, out int sec, out int ms);
			Assert.Equal(2020, year);
			Assert.Equal(7, month);
			Assert.Equal(14, day);
			Assert.Equal(16, hour);
			Assert.Equal(24, min);
			Assert.Equal(59, sec);
			Assert.Equal(129, ms);
			Assert.Equal(2020, dt.Year);
			Assert.Equal(7, dt.Month);
			Assert.Equal(14, dt.Day);
			Assert.Equal(16, dt.Hour);
			Assert.Equal(24, dt.Minute);
			Assert.Equal(59, dt.Second);
			Assert.Equal(129, dt.Millisecond);
		}
		[Fact]
		public void Truncate()
		{
			UtcDateTime dt = new(2020, 7, 14, 16, 24, 59, 129);
			Assert.Equal(dt, dt.Truncate(DateTimePart.Millisecond));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 24, 59), dt.Truncate(DateTimePart.Second));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 24, 0), dt.Truncate(DateTimePart.Minute));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 0, 0), dt.Truncate(DateTimePart.Hour));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 0, 0, 0), dt.Truncate(DateTimePart.Day));
			Assert.Equal(new UtcDateTime(2020, 7, 1, 0, 0, 0), dt.Truncate(DateTimePart.Month));
			Assert.Equal(new UtcDateTime(2020, 1, 1, 0, 0, 0), dt.Truncate(DateTimePart.Year));
		}
		[Fact]
		public void DatesSurviveRoundTrip()
		{
			IntRng rng = new();
			for (int year = 1; year <= 9999; year++)
			{
				for (int month = 1; month <= 12; month++)
				{
					int daysInMonth = DateTime.DaysInMonth(year, month);
					for (int day = 1; day <= daysInMonth; day++)
					{
						// Takes too long to do all the hours/mins/secs/millis, so just use a random value for each one
						int hour = rng.Next(0, 24);
						int minute = rng.Next(0, 60);
						int second = rng.Next(0, 60);
						int millis = rng.Next(0, 1000);

						UtcDateTime dt = new(year, month, day, hour, minute, second, millis);
						dt.Deconstruct(out int y, out int mon, out int d, out int h, out int min, out int s, out int ms);
						Assert.Equal(year, y);
						Assert.Equal(month, mon);
						Assert.Equal(day, d);
						Assert.Equal(hour, h);
						Assert.Equal(minute, min);
						Assert.Equal(second, s);
						Assert.Equal(millis, ms);
					}
				}
			}
		}
		[Fact]
		public void AddDaysAndDayOfYear()
		{
			UtcDateTime dt = new(1999, 1, 1);
			Assert.Equal(1, dt.DayOfYear);
			for (int i = 1; i < 365; i++)
			{
				Assert.Equal(i + 1, dt.AddDays(i).DayOfYear);
			}

			// Leap year, should go up to 366
			dt = new UtcDateTime(2000, 1, 1);
			Assert.Equal(1, dt.DayOfYear);
			for (int i = 1; i < 366; i++)
			{
				Assert.Equal(i + 1, dt.AddDays(i).DayOfYear);
			}
		}
		[Fact]
		public void AddYears()
		{
			UtcDateTime dt = new(2004, 2, 29);
			UtcDateTime result = dt.AddYears(1);
			Assert.Equal(2005, result.Year);
			Assert.Equal(2, result.Month);
			Assert.Equal(28, result.Day);
			Assert.Equal(0, result.Hour);
			Assert.Equal(0, result.Minute);
			Assert.Equal(0, result.Second);
			Assert.Equal(0, result.Millisecond);

			dt = new UtcDateTime(1, 1, 31);
			for (int i = 1; i < 9998; i++)
			{
				result = dt.AddYears(i);
				result.GetDateParts(out int y, out int m, out int d);
				Assert.Equal(i + 1, y);
				Assert.Equal(1, m);
				Assert.Equal(DateTime.DaysInMonth(i + 1, 1), d);
			}
		}
		[Fact]
		public void AddPositiveMonths()
		{
			UtcDateTime dt = new(2000, 1, 31);
			UtcDateTime result = dt.AddMonths(0);
			Assert.Equal(dt, result);

			result = dt.AddMonths(1);
			Assert.Equal(new UtcDateTime(2000, 2, 29), result);

			result = dt.AddMonths(2);
			Assert.Equal(new UtcDateTime(2000, 3, 31), result);

			result = dt.AddMonths(3);
			Assert.Equal(new UtcDateTime(2000, 4, 30), result);

			result = dt.AddMonths(4);
			Assert.Equal(new UtcDateTime(2000, 5, 31), result);

			result = dt.AddMonths(5);
			Assert.Equal(new UtcDateTime(2000, 6, 30), result);

			result = dt.AddMonths(6);
			Assert.Equal(new UtcDateTime(2000, 7, 31), result);

			result = dt.AddMonths(7);
			Assert.Equal(new UtcDateTime(2000, 8, 31), result);

			result = dt.AddMonths(8);
			Assert.Equal(new UtcDateTime(2000, 9, 30), result);

			result = dt.AddMonths(9);
			Assert.Equal(new UtcDateTime(2000, 10, 31), result);

			result = dt.AddMonths(10);
			Assert.Equal(new UtcDateTime(2000, 11, 30), result);

			result = dt.AddMonths(11);
			Assert.Equal(new UtcDateTime(2000, 12, 31), result);

			result = dt.AddMonths(12);
			Assert.Equal(new UtcDateTime(2001, 1, 31), result);

			result = dt.AddMonths(13);
			Assert.Equal(new UtcDateTime(2001, 2, 28), result);

			result = dt.AddMonths(14);
			Assert.Equal(new UtcDateTime(2001, 3, 31), result);

			result = dt.AddMonths(15);
			Assert.Equal(new UtcDateTime(2001, 4, 30), result);

			result = dt.AddMonths(16);
			Assert.Equal(new UtcDateTime(2001, 5, 31), result);

			result = dt.AddMonths(17);
			Assert.Equal(new UtcDateTime(2001, 6, 30), result);

			result = dt.AddMonths(18);
			Assert.Equal(new UtcDateTime(2001, 7, 31), result);

			result = dt.AddMonths(19);
			Assert.Equal(new UtcDateTime(2001, 8, 31), result);

			result = dt.AddMonths(20);
			Assert.Equal(new UtcDateTime(2001, 9, 30), result);

			result = dt.AddMonths(21);
			Assert.Equal(new UtcDateTime(2001, 10, 31), result);

			result = dt.AddMonths(22);
			Assert.Equal(new UtcDateTime(2001, 11, 30), result);

			result = dt.AddMonths(23);
			Assert.Equal(new UtcDateTime(2001, 12, 31), result);

			result = dt.AddMonths(24);
			Assert.Equal(new UtcDateTime(2002, 1, 31), result);
		}
		[Fact]
		public void AddNegativeMonths()
		{
			UtcDateTime dt = new(2000, 1, 31);
			UtcDateTime result = dt.AddMonths(0);
			Assert.Equal(dt, result);

			result = dt.AddMonths(-1);
			Assert.Equal(new UtcDateTime(1999, 12, 31), result);

			result = dt.AddMonths(-2);
			Assert.Equal(new UtcDateTime(1999, 11, 30), result);

			result = dt.AddMonths(-3);
			Assert.Equal(new UtcDateTime(1999, 10, 31), result);

			result = dt.AddMonths(-4);
			Assert.Equal(new UtcDateTime(1999, 9, 30), result);

			result = dt.AddMonths(-5);
			Assert.Equal(new UtcDateTime(1999, 8, 31), result);

			result = dt.AddMonths(-6);
			Assert.Equal(new UtcDateTime(1999, 7, 31), result);

			result = dt.AddMonths(-7);
			Assert.Equal(new UtcDateTime(1999, 6, 30), result);

			result = dt.AddMonths(-8);
			Assert.Equal(new UtcDateTime(1999, 5, 31), result);

			result = dt.AddMonths(-9);
			Assert.Equal(new UtcDateTime(1999, 4, 30), result);

			result = dt.AddMonths(-10);
			Assert.Equal(new UtcDateTime(1999, 3, 31), result);

			result = dt.AddMonths(-11);
			Assert.Equal(new UtcDateTime(1999, 2, 28), result);

			result = dt.AddMonths(-12);
			Assert.Equal(new UtcDateTime(1999, 1, 31), result);

			result = dt.AddMonths(-13);
			Assert.Equal(new UtcDateTime(1998, 12, 31), result);

			result = dt.AddMonths(-14);
			Assert.Equal(new UtcDateTime(1998, 11, 30), result);

			result = dt.AddMonths(-15);
			Assert.Equal(new UtcDateTime(1998, 10, 31), result);

			result = dt.AddMonths(-16);
			Assert.Equal(new UtcDateTime(1998, 9, 30), result);

			result = dt.AddMonths(-17);
			Assert.Equal(new UtcDateTime(1998, 8, 31), result);

			result = dt.AddMonths(-18);
			Assert.Equal(new UtcDateTime(1998, 7, 31), result);

			result = dt.AddMonths(-19);
			Assert.Equal(new UtcDateTime(1998, 6, 30), result);

			result = dt.AddMonths(-20);
			Assert.Equal(new UtcDateTime(1998, 5, 31), result);

			result = dt.AddMonths(-21);
			Assert.Equal(new UtcDateTime(1998, 4, 30), result);

			result = dt.AddMonths(-22);
			Assert.Equal(new UtcDateTime(1998, 3, 31), result);

			result = dt.AddMonths(-23);
			Assert.Equal(new UtcDateTime(1998, 2, 28), result);

			result = dt.AddMonths(-24);
			Assert.Equal(new UtcDateTime(1998, 1, 31), result);

			result = dt.AddMonths(-25);
			Assert.Equal(new UtcDateTime(1997, 12, 31), result);
		}
	}
}
