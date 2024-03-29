﻿namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;
	public sealed class ParsingAndFormatting
	{
		public static readonly Tz tzp10_00 = Tz.TryCreate(10, 0).ValueOrException();
		public static readonly Tz tzn10_00 = Tz.TryCreate(-10, 0).ValueOrException();
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
			Assert.Equal(DotNetTime.DaysPer4Years, daysPer4Years);
			Assert.Equal(DotNetTime.DaysPer100Years, daysPer100Years);
			Assert.Equal(DotNetTime.DaysPer400Years, daysPer400Years);

			long epochDays = 0;
			for (int i = 1; i < 1970; i++)
			{
				epochDays += DateTime.IsLeapYear(i) ? 366 : 365;
			}

			Assert.Equal(DotNetTime.UnixEpochTicks, UtcDateTime.UnixEpoch.Ticks);
			UtcDateTime minValue = default;
			Assert.Equal(0, minValue.Ticks);
			Assert.Equal(0, UtcDateTime.MinValue.Ticks);
			Assert.Equal(DotNetTime.MaxTicks, UtcDateTime.MaxValue.Ticks);
			Assert.Equal(minValue, new());
		}
		[Fact]
		public void ParseRfc3339Strings()
		{
			Assert.Equal(new UtcDateTime(1234, 5, 15, 10, 20, 30, 0), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30Z").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 10, 20, 30, 100), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.1Z").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 10, 20, 30, 120), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.12Z").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 10, 20, 30, 123), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.123Z").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 0, 20, 30, 0), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30+10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 0, 20, 30, 300), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.3+10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 0, 20, 30, 320), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.32+10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 0, 20, 30, 321), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.321+10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 20, 20, 30, 0), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30-10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 20, 20, 30, 400), UtcDateTime.TryParseRfc3339String("1234-05-15T10:20:30.4-10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 20, 20, 30, 450), UtcDateTime.TryParseRfc3339String("1234-05-15t10:20:30.45-10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 20, 20, 30, 456), UtcDateTime.TryParseRfc3339String("1234-05-15t10:20:30.456-10:00").ValueOrException());
			Assert.Equal(new UtcDateTime(1234, 5, 15, 20, 20, 30, 456), UtcDateTime.TryParseRfc3339String("1234-05-15 10:20:30.456-10:00", allowSpaceInsteadOfT: true).ValueOrException());
		}
		[Fact]
		public void ParseIso8601Strings()
		{
			Assert.Equal(new UtcDateTime(2019, 12, 30), UtcDateTime.TryParseIso8601String("2020-W01", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2019, 12, 30), UtcDateTime.TryParseIso8601String("2020-W01-1", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2019, 12, 31), UtcDateTime.TryParseIso8601String("2020-W01-2", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 1, 1), UtcDateTime.TryParseIso8601String("2020-W01-3", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 1, 2), UtcDateTime.TryParseIso8601String("2020-W01-4", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 1, 3), UtcDateTime.TryParseIso8601String("2020-W01-5", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 1, 4), UtcDateTime.TryParseIso8601String("2020-W01-6", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 1, 5), UtcDateTime.TryParseIso8601String("2020-W01-7", Tz.Utc).ValueOrException());

			Assert.Equal("When parsing a UtcDateTime, a date part is required. If you only want to parse a time, use the Iso8601 class directly.", UtcDateTime.TryParseIso8601String("T12:00:00Z").ErrorOr(null));

			// Empty string of course not good
			Assert.Equal("String was empty", UtcDateTime.TryParseIso8601String("", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse year because it was not 4 digits long. String: 2", UtcDateTime.TryParseIso8601String("2", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse year because it was not 4 digits long. String: 20", UtcDateTime.TryParseIso8601String("20", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse year because it was not 4 digits long. String: 202", UtcDateTime.TryParseIso8601String("202", Tz.Utc).ErrorOr(null));

			// Only year is not enough
			Assert.Equal("String only consists of a year: 2020", UtcDateTime.TryParseIso8601String("2020", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse Months/Weeks/Ordinal Days because it was not at least 2 digits long. String: 2020-", UtcDateTime.TryParseIso8601String("2020-", Tz.Utc).ErrorOr(null));

			// 5 digits, not good
			Assert.Equal("Failed to parse Months/Weeks/Ordinal Days because it was not at least 2 digits long. String: 20201", UtcDateTime.TryParseIso8601String("20201", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse Months/Weeks/Ordinal Days because it was not at least 2 digits long. String: 2020-1", UtcDateTime.TryParseIso8601String("2020-1", Tz.Utc).ErrorOr(null));

			// Not actually valid; yyyyMM can be confused with yyyy-MM, so it isn't allowed
			Assert.Equal("Parsed only a year and month without a separator, which ISO-8601 disallows because it can be confused with yyMMdd. Only yyyy-MM is valid, not yyyyMM. String: 202011", UtcDateTime.TryParseIso8601String("202011", Tz.Utc).ErrorOr(null));
			// This is valid; yyyy-MM is not ambiguous unlike yyyyMM
			Assert.Equal(new UtcDateTime(2020, 11, 1), UtcDateTime.TryParseIso8601String("2020-11", Tz.Utc).ValueOrException());

			// Interpreted as Ordinal (yyyy-ddd)
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020167", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020-167", Tz.Utc).ValueOrException());

			// Non-digits
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: X67. String: 2020-X67", UtcDateTime.TryParseIso8601String("2020-X67", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: 1X7. String: 2020-1X7", UtcDateTime.TryParseIso8601String("2020-1X7", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: 16X. String: 2020-16X", UtcDateTime.TryParseIso8601String("2020-16X", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: X67. String: 2020-X67T23", UtcDateTime.TryParseIso8601String("2020-X67T23", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: 1X7. String: 2020-1X7T23:10", UtcDateTime.TryParseIso8601String("2020-1X7T23:10", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: 16X. String: 2020-16XT23:10:52", UtcDateTime.TryParseIso8601String("2020-16XT23:10:52", Tz.Utc).ErrorOr(null));

			// ISO-8601 weeks
			Assert.Equal(new UtcDateTime(2020, 3, 2), UtcDateTime.TryParseIso8601String("2020W10", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 3, 2), UtcDateTime.TryParseIso8601String("2020-w10", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 3, 2), UtcDateTime.TryParseIso8601String("2020w101", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 3, 2), UtcDateTime.TryParseIso8601String("2020-W10-1", Tz.Utc).ValueOrException());

			// We don't do yyyy-MM-, because that gets interpreted as ordinal days, because it's only 3 chars until T or the end of the string
			Assert.Equal("Failed to parse ordinal days because Found a non-latin digit in the string: 06-. String: 2020-06-", UtcDateTime.TryParseIso8601String("2020-06-", Tz.Utc).ErrorOr(null));
			Assert.Equal("Days part was not at least 2 digits long. String: 2020-06-1", UtcDateTime.TryParseIso8601String("2020-06-1", Tz.Utc).ErrorOr(null));

			// Interpreted as yyyy-MM-dd 00:00:00
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("20200615", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15), UtcDateTime.TryParseIso8601String("2020-06-15", Tz.Utc).ValueOrException());

			// Not allowed when we require a timezone
			Assert.Equal("No timezone was found when parsing and no default timezone was provided. String: 2020-06-15", UtcDateTime.TryParseIso8601String("2020-06-15").ErrorOr(null));

			// Non-digits
			Assert.Equal("Failed to parse day because Found a non-latin digit in the string: X5. String: 202006X5", UtcDateTime.TryParseIso8601String("202006X5", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse day because Found a non-latin digit in the string: X5. String: 2020-06-X5", UtcDateTime.TryParseIso8601String("2020-06-X5", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse day because Found a non-latin digit in the string: 1X. String: 2020061X", UtcDateTime.TryParseIso8601String("2020061X", Tz.Utc).ErrorOr(null));
			Assert.Equal("Failed to parse day because Found a non-latin digit in the string: 1X. String: 2020-06-1X", UtcDateTime.TryParseIso8601String("2020-06-1X", Tz.Utc).ErrorOr(null));

			// Inconsistent separators
			Assert.Equal("Separator between Year/Month was present, but separator between Month/Day was missing. String: 2020-0615", UtcDateTime.TryParseIso8601String("2020-0615", Tz.Utc).ErrorOr(null));
			Assert.Equal("Separator between Year/Month was missing, but separator between Month/Day was present. String: 202006-15", UtcDateTime.TryParseIso8601String("202006-15", Tz.Utc).ErrorOr(null));

			// Can't have the timezone yet
			Assert.Equal("Date and Time separator T was expected at index 8. String: 20200615Z", UtcDateTime.TryParseIso8601String("20200615Z").ErrorOr(null));
			Assert.Equal("Date and Time separator T was expected at index 10. String: 2020-06-15Z", UtcDateTime.TryParseIso8601String("2020-06-15Z").ErrorOr(null));

			// Can't end with T, and need at least 2 digits after that
			Assert.Equal("Hours part was not 2 digits long. String: 20200615T", UtcDateTime.TryParseIso8601String("20200615T", Tz.Utc).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long. String: 20200615t2", UtcDateTime.TryParseIso8601String("20200615t2", Tz.Utc).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long. String: 2020-06-15t", UtcDateTime.TryParseIso8601String("2020-06-15t", Tz.Utc).ErrorOr(null));
			Assert.Equal("Hours part was not 2 digits long. String: 2020-06-15T2", UtcDateTime.TryParseIso8601String("2020-06-15T2", Tz.Utc).ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:00:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("20200615T23", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("20200615t23", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15t23", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23Z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23+10:00", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23", tzp10_00).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 16, 9, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23-10:00", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 16, 9, 0, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23", tzn10_00).ValueOrException());

			// Need at least 2 digits for the minute
			Assert.Equal("Minutes part was not 2 digits long. String: 2020-06-15T23:", UtcDateTime.TryParseIso8601String("2020-06-15T23:", Tz.Utc).ErrorOr(null));
			Assert.Equal("Minutes part was not 2 digits long. String: 20200615t231", UtcDateTime.TryParseIso8601String("20200615t231", Tz.Utc).ErrorOr(null));
			Assert.Equal("Minutes part was not 2 digits long. String: 2020-06-15t23:1", UtcDateTime.TryParseIso8601String("2020-06-15t23:1", Tz.Utc).ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:mm:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("20200615T2310", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23:10", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), UtcDateTime.TryParseIso8601String("2020-06-15T23:10Z", Tz.Utc).ValueOrException());

			// Need at least 2 digits for the second
			Assert.Equal("Seconds part was not 2 digits long. String: 2020-06-15T23:10:", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:").ErrorOr(null));
			Assert.Equal("Seconds part was not 2 digits long. String: 20200615T23105", UtcDateTime.TryParseIso8601String("20200615T23105").ErrorOr(null));
			Assert.Equal("Seconds part was not 2 digits long. String: 2020-06-15t23:10:5", UtcDateTime.TryParseIso8601String("2020-06-15t23:10:5").ErrorOr(null));

			// Interpreted as yyyy-MM-dd HH:mm:ss
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("20200615T231052", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("20200615T231052Z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("     2020-06-15t23:10:52    ", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52Z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52+10:00", Tz.Utc).ValueOrException());

			// Inconsistent separators
			Assert.Equal("Separator between Hour/Minute was present, but separator between Minute/Second was missing", UtcDateTime.TryParseIso8601String("2020-06-15T23:1052").ErrorOr(null));
			Assert.Equal("Separator between Hour/Minute was missing, but separator between Minute/Second was present", UtcDateTime.TryParseIso8601String("2020-06-15T2310:52").ErrorOr(null));

			// Need at least 1 millisecond digit
			Assert.Equal("Milliseconds separator was found but no milliseconds were found. String: 20200615T231052.", UtcDateTime.TryParseIso8601String("20200615T231052.").ErrorOr(null));
			Assert.Equal("Milliseconds separator was found but no milliseconds were found. String: 2020-06-15T23:10:52.", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.").ErrorOr(null));


			// Milliseconnndddssss
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 100), UtcDateTime.TryParseIso8601String("20200615T231052.1", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 120), UtcDateTime.TryParseIso8601String("20200615T231052.12", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052,1234", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052,12345", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123456", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 100), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.1", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 120), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52,123", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52,1234", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12345", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123456", Tz.Utc).ValueOrException());

			// Milliseconds have to be all latin digits
			Assert.Equal("Milliseconds separator was found but no milliseconds were found. String: 20200615T231052.", UtcDateTime.TryParseIso8601String("20200615T231052.").ErrorOr(null));
			Assert.Equal("Milliseconds separator was found but no milliseconds were found. String: 2020-06-15T23:10:52.", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.").ErrorOr(null));

			// Everything with the Z timezone
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123Z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123Z", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123z", Tz.Utc).ValueOrException());

			// Everything with an implicit timezone of +10:00
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123", tzp10_00).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123", tzp10_00).ValueOrException());

			// Timezone offset of +10:00 and +10:30
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("20200615t231052.123+10", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), UtcDateTime.TryParseIso8601String("20200615T231052.123+1030", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123+10:30", Tz.Utc).ValueOrException());

			// Max and min offsets
			Assert.Equal(new UtcDateTime(2020, 6, 15, 9, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123+14:00", Tz.Utc).ValueOrException());
			Assert.Equal(new UtcDateTime(2020, 6, 16, 11, 10, 52, 123), UtcDateTime.TryParseIso8601String("2020-06-15t23:10:52.123-12:00", Tz.Utc).ValueOrException());

			// Timezone char that's wrong
			Assert.Equal("Timezone designator was not valid. String: 20200615T231052.123X", UtcDateTime.TryParseIso8601String("20200615T231052.123X").ErrorOr(null));
			Assert.Equal("Timezone designator was not valid. String: 2020-06-15T23:10:52.123Y", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123Y").ErrorOr(null));

			// Timezone hours wrong length
			Assert.Equal("Timezone hours part was not 2 digits long. String: 20200615T231052.123+", UtcDateTime.TryParseIso8601String("20200615T231052.123+").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long. String: 20200615T231052.123-", UtcDateTime.TryParseIso8601String("20200615T231052.123-").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long. String: 20200615T231052.123+1", UtcDateTime.TryParseIso8601String("20200615T231052.123+1").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long. String: 2020-06-15T23:10:52.123+", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long. String: 2020-06-15T23:10:52.123-", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123-").ErrorOr(null));
			Assert.Equal("Timezone hours part was not 2 digits long. String: 2020-06-15T23:10:52.123+1", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+1").ErrorOr(null));

			// Timezone minutes wrong
			Assert.Equal("Timezone minutes part was not 2 digits long. String: 20200615T231052.123+103", UtcDateTime.TryParseIso8601String("20200615T231052.123+103").ErrorOr(null));
			Assert.Equal("Timezone minutes part was not 2 digits long. String: 2020-06-15T23:10:52.123+10:3", UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10:3").ErrorOr(null));
		}
		[Fact]
		public void DecimalPlaces()
		{
			UtcDateTime udt = new UtcDateTime(2022, 1, 2, 3, 4, 5).AddTicks(1234567);

			Assert.Equal("Decimal places must be at least 1 if fractions of a second are being written", Assert.Throws<NoValueException>(() => Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, 0).ValueOrException()).Message);

			Iso8601Format fmt;
			
			int d = 1;

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.1Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.12Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.123Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.1234Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.12345Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.123456Z", udt.ToIso8601StringAsFormat(fmt, null));

			fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d++).ValueOrException();
			Assert.Equal("2022-01-02T03:04:05.1234567Z", udt.ToIso8601StringAsFormat(fmt, null));

			for (d = 8; d < 100; d++)
			{
				fmt = Iso8601Format.TryCreate(Iso8601Parts.Format_ExtendedFormat_UtcTz, d).ValueOrException();
				string expected = string.Concat("2022-01-02T03:04:05.1234567", new string('0', d - 7) , "Z");
				Assert.Equal(expected, udt.ToIso8601StringAsFormat(fmt, null));
			}
		}
		[Fact]
		public void Iso8601WeekFormatting()
		{
			Iso8601Format fmt = Iso8601Format.TryCreate(Iso8601Parts.YearWeekDay | Iso8601Parts.Separator_Date, 0).ValueOrException();
			UtcDateTime udt = new(2019, 12, 30);
			Span<char> expected = stackalloc char[10];
			expected[0] = '2';
			expected[1] = '0';
			expected[2] = '2';
			expected[3] = '0';
			expected[4] = '-';
			expected[5] = 'W';
			expected[8] = '-';
			Span<char> actual = stackalloc char[10];
			for (uint week = 1; week <= (uint)IsoYearWeek.WeeksInYear(2020); week++)
			{
				Formatting.Write2Digits(week, expected, 6);
				for (int day = 1; day < 8; day++)
				{
					expected[9] = (char)('0' + day);

					udt.Format(actual, Tz.Utc, fmt);

					Assert.Equal(new string(expected), new string(actual));

					udt = udt.AddDays(1);
				}
			}
		}
		[Fact]
		public void BadFormats()
		{
			Assert.Equal("Decimal places must be at least 1 if fractions of a second are being written",
				Iso8601Format.TryCreate(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, decimalPlaces: 0).ErrorOr(null));

			Assert.Equal("Decimal places must be at least 1 if fractions of a second are being written",
				Iso8601Format.TryCreate(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, decimalPlaces: 0).ErrorOr(null));
		}
		[Fact]
		public void BuiltInFormats()
		{
			UtcDateTime dt = new(2020, 6, 5, 3, 0, 52, 012);
			Assert.Equal("2020-06-05T03:00:52.012+00:00", dt.ToIso8601StringAsFormat(Iso8601Format.ExtendedFormat_FullTz, Tz.Utc));
			Assert.Equal("20200605T030052.012Z", dt.ToIso8601StringAsFormat(Iso8601Format.BasicFormat_UtcTz, Tz.Utc));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringAsFormat(Iso8601Format.ExtendedFormat_UtcTz, Tz.Utc));
			Assert.Equal("2020-06-05", dt.ToIso8601StringAsFormat(Iso8601Format.DateOnly, Tz.Utc));
			Assert.Equal("20200605", dt.ToIso8601StringAsFormat(Iso8601Format.DateOnlyWithoutSeparators, Tz.Utc));
			Assert.Equal("2020-157", dt.ToIso8601StringAsFormat(Iso8601Format.DateOrdinal, Tz.Utc));
			UtcDateTime dt2 = new UtcDateTime(2019, 1, 1).AddDays(156);
			Assert.Equal("2019-157", dt2.ToIso8601StringAsFormat(Iso8601Format.DateOrdinal, Tz.Utc));
			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601StringAsFormat(Iso8601Format.ExtendedFormat_NoFractional_UtcTz, Tz.Utc));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringAsFormat(Iso8601Format.ExtendedFormat_UtcTz, Tz.Utc));

			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToString());

			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringUtc(extended: true, decimalPlaces: 3));
			Assert.Equal("20200605T030052.012Z", dt.ToIso8601StringUtc(extended: false, decimalPlaces: 3));
			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601StringUtc(extended: true, decimalPlaces: 0));
			Assert.Equal("20200605T030052Z", dt.ToIso8601StringUtc(extended: false, decimalPlaces: 0));

			Assert.Equal("2020-06-05T13:00:52.012+10:00", dt.ToIso8601StringTz(extended: true, decimalPlaces: 3, tzp10_00));
			Assert.Equal("20200605T133052.012+1030", dt.ToIso8601StringTz(extended: false, decimalPlaces: 3, Tz.TryCreate(10, 30).ValueOrException()));
			Assert.Equal("2020-06-05T14:00:52+11:00", dt.ToIso8601StringTz(extended: true, decimalPlaces: 0, Tz.TryCreate(11, 0).ValueOrException()));
			Assert.Equal("20200605T150052+1200", dt.ToIso8601StringTz(extended: false, decimalPlaces: 0, Tz.TryCreate(12, 0).ValueOrException()));
		}
		[Fact]
		public void UtcDateTimeToString()
		{
			Iso8601Format weekFmt = Iso8601Format.TryCreate(Iso8601Parts.YearWeekDay | Iso8601Parts.Separator_Date, 0).ValueOrException();
			Assert.Equal("2019-W52-7", new UtcDateTime(2019, 12, 29).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-1", new UtcDateTime(2019, 12, 30).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-2", new UtcDateTime(2019, 12, 31).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-3", new UtcDateTime(2020, 1, 1).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-4", new UtcDateTime(2020, 1, 2).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-5", new UtcDateTime(2020, 1, 3).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-6", new UtcDateTime(2020, 1, 4).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W01-7", new UtcDateTime(2020, 1, 5).ToIso8601StringAsFormat(weekFmt, Tz.Utc));
			Assert.Equal("2020-W02-1", new UtcDateTime(2020, 1, 6).ToIso8601StringAsFormat(weekFmt, Tz.Utc));

			UtcDateTime dt = new UtcDateTime(2020, 6, 5, 3, 0, 52, 012).AddTicks(3456);
			Tz tzUtc = Tz.Utc;

			TestFormat("20200605T03:00:52Z", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Separator_Time, tzUtc, 0);

			TestFormat("2020-06-05T03", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("2020-06-05T03Z", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("2020-06-05T03+00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("2020-06-05T03+00:00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);

			TestFormat("2020-06-05T03:00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("2020-06-05T03:00Z", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("2020-06-05T03:00+00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("2020-06-05T03:00+00:00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);

			TestFormat("2020-06-05T03:00:52", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("2020-06-05T03:00:52Z", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("2020-06-05T03:00:52+00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("2020-06-05T03:00:52+00:00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);

			TestFormat("2020-06-05T03:00:52.012", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All, tzUtc, 3);
			TestFormat("2020-06-05T03:00:52.012Z", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 3);
			TestFormat("2020-06-05T03:00:52.012+00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 3);
			TestFormat("2020-06-05T03:00:52.012+00:00", dt,Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 3);

			TestFormat("2020-06-05T03:00:52.0Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 1);
			TestFormat("2020-06-05T03:00:52.01Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 2);
			TestFormat("2020-06-05T03:00:52.012Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 3);
			TestFormat("2020-06-05T03:00:52.0123Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 4);
			TestFormat("2020-06-05T03:00:52.01234Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 5);
			TestFormat("2020-06-05T03:00:52.012345Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 6);
			TestFormat("2020-06-05T03:00:52.0123456Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 7);
			TestFormat("2020-06-05T03:00:52.01234560Z", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, decimalPlaces: 8);

			TestFormat("2020-06-05T03:00:52.0+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 1);
			TestFormat("2020-06-05T03:00:52.01+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 2);
			TestFormat("2020-06-05T03:00:52.012+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 3);
			TestFormat("2020-06-05T03:00:52.0123+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 4);
			TestFormat("2020-06-05T03:00:52.01234+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 5);
			TestFormat("2020-06-05T03:00:52.012345+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 6);
			TestFormat("2020-06-05T03:00:52.0123456+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 7);
			TestFormat("2020-06-05T03:00:52.01234560+00:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, decimalPlaces: 8);

			TestFormat("2020-06-05T06:00:52.012+03:00", dt, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, Tz.TryCreate(3, 0).ValueOrException(), 3);

			TestFormat("T03", dt,Iso8601Parts.Hour | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("T03Z", dt,Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T03+00", dt,Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T03+00:00", dt,Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T03:00", dt,Iso8601Parts.HourMinute | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("T03:00Z", dt,Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T03:00+00", dt,Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T03:00+00:00", dt,Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T03:00:52", dt,Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All, tzUtc, 0);
			TestFormat("T03:00:52Z", dt,Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T03:00:52+00", dt,Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T03:00:52+00:00", dt,Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T03:00:52.012", dt,Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All, tzUtc, 3);
			TestFormat("T03:00:52.012Z", dt,Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc, tzUtc, 3);
			TestFormat("T03:00:52.012+00", dt,Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour, tzUtc, 3);
			TestFormat("T03:00:52.012+00:00", dt,Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute, tzUtc, 3);

			TestFormat("T03", dt, Iso8601Parts.Hour, tzUtc, 0);
			TestFormat("T03Z", dt, Iso8601Parts.Hour | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T03+00", dt, Iso8601Parts.Hour | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T03+0000", dt, Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T0300", dt, Iso8601Parts.HourMinute, tzUtc, 0);
			TestFormat("T0300Z", dt, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T0300+00", dt, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T0300+0000", dt, Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T030052", dt, Iso8601Parts.HourMinuteSecond, tzUtc, 0);
			TestFormat("T030052Z", dt, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc, tzUtc, 0);
			TestFormat("T030052+00", dt, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Hour, tzUtc, 0);
			TestFormat("T030052+0000", dt, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute, tzUtc, 0);
			TestFormat("T030052.012", dt, Iso8601Parts.HourMinuteSecondFractional, tzUtc, 3);
			TestFormat("T030052.012Z", dt, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Utc, tzUtc, 3);
			TestFormat("T030052.012+00", dt, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Hour, tzUtc, 3);
			TestFormat("T030052.012+0000", dt, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute, tzUtc, 3);

			dt = new(2020, 6, 5, 0, 0, 0);
			TestFormat("2020-06-05", dt, Iso8601Format.DateOnly, Tz.Utc, 0);
			TestFormat("2020-06-04", dt, Iso8601Format.DateOnly, Tz.TryCreate(-10, 0).ValueOrException(), 0);
			dt = new(2020, 6, 5, 20, 0, 0);
			TestFormat("2020-06-06", dt, Iso8601Format.DateOnly, Tz.TryCreate(10, 0).ValueOrException(), 0);
		}
		private static void TestFormat(string expected, UtcDateTime udt, Iso8601Parts parts, Tz? timezone, int decimalPlaces)
		{
			// Make sure TryCreate works
			Iso8601Format format = Iso8601Format.TryCreate(parts, decimalPlaces).ValueOrException();
			TestFormat(expected, udt, format, timezone, decimalPlaces);
		}
		private static void TestFormat(string expected, UtcDateTime udt, Iso8601Format format, Tz? timezone, int decimalPlaces)
		{
			Span<char> actual = stackalloc char[format.LengthRequired];

			// Make sure WriteString returns the number of characters it said it needed
			Assert.Equal(format.LengthRequired, format.WriteString(actual, udt.Ticks, timezone));

			// And of course check the strings
			Assert.Equal(expected, new string(actual));
			Assert.Equal(expected, format.CreateString(udt.Ticks, timezone));
		}
	}
}
