namespace MichMcb.CsExt.Test
{
	using System;
	using Xunit;
	public sealed  class UtcDateTimeTest
	{
		// TODO get as much coverage as possible on the methods and properties

		//private readonly ITestOutputHelper output;
		//public UtcDateTimeTest(ITestOutputHelper output)
		//{
		//	this.output = output;
		//}
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
			Assert.Equal(Dates.DaysPer4Years, daysPer4Years);
			Assert.Equal(Dates.DaysPer100Years, daysPer100Years);
			Assert.Equal(Dates.DaysPer400Years, daysPer400Years);

			long epochDays = 0;
			for (int i = 1; i < 1970; i++)
			{
				epochDays += DateTime.IsLeapYear(i) ? 366 : 365;
			}

			Assert.Equal(Dates.UnixEpochMillis, UtcDateTime.UnixEpoch.TotalMilliseconds);
			UtcDateTime minValue = new UtcDateTime();
			Assert.Equal(0, minValue.TotalMilliseconds);
			Assert.Equal(0, UtcDateTime.MinValue.TotalMilliseconds);
			Assert.Equal(Dates.MaxMillis, UtcDateTime.MaxValue.TotalMilliseconds);
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
			// Don't hate me for all these inline assignments...I changed the method to return a Maybe<UtcDateTime, string>, and this was the lowest-effort way of fixing these tests

			Maybe<UtcDateTime, string> maybe = UtcDateTime.TryParseIso8601String("");
			Assert.False(maybe.Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("202").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20201").Ok);
			// Not actually valid; yyyyMM can be confused with yyyy-MM, so it isn't allowed
			Assert.False(UtcDateTime.TryParseIso8601String("202011").Ok);
			// Interpreted as Ordinal
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020165", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 13), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T2").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T23", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T2310", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T23105").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.1", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 1), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.12", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 12), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.1234", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.12345", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123456", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123Z", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123")).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.123X").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.123+").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.123-").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.123+1").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123+10", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("20200615T231052.123+103").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("20200615T231052.123+1030", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), maybe.ValueOr(default));

			Assert.False(UtcDateTime.TryParseIso8601String("").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("20").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("202").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-1").Ok);
			// This is valid; yyyy-MM is not ambiguous unlike yyyyMM
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-11", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 11, 1), maybe.ValueOr(default));
			// Interpreted as Ordinal
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-165", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 13), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-1").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T2").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 0, 0), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:1").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 0), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:5").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.1", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 1), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 12), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.1234", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.12345", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123456", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123Z", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 23, 10, 52, 123), maybe.ValueOr(default));
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123")).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123X").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123-").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+1").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 13, 10, 52, 123), maybe.ValueOr(default));
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10:").Ok);
			Assert.False(UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10:3").Ok);
			Assert.True((maybe = UtcDateTime.TryParseIso8601String("2020-06-15T23:10:52.123+10:30", TimeSpan.Zero)).Ok);
			Assert.Equal(new UtcDateTime(2020, 6, 15, 12, 40, 52, 123), maybe.ValueOr(default));
		}
		[Fact]
		public void UtcDateTimeToString()
		{
			UtcDateTime dt = new UtcDateTime(2020, 6, 5, 3, 0, 52, 012);
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToString());
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringUtc());
			Assert.Equal("2020-06-05T03:00:52.012+00:00", dt.ToIso8601StringUtc(Iso8601Parts.Format_ExtendedFormat_FullTz));
			Assert.Equal("20200605T030052.012Z", dt.ToIso8601StringUtc(Iso8601Parts.Format_BasicFormat_UtcTz));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringUtc(Iso8601Parts.Format_ExtendedFormat_UtcTz));
			Assert.Equal("2020-06-05", dt.ToIso8601StringUtc(Iso8601Parts.Format_DateOnly));
			Assert.Equal("20200605", dt.ToIso8601StringUtc(Iso8601Parts.Format_DateOnlyWithoutSeparators));
			Assert.Equal("2020-157", dt.ToIso8601StringUtc(Iso8601Parts.Format_DateOrdinal));
			Assert.Equal("--06-05", dt.ToIso8601StringUtc(Iso8601Parts.Format_VcfUnknownYear));

			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601StringUtc(Iso8601Parts.Format_ExtendedFormat_NoMillis_UtcTz));
			Assert.Equal("20200605T03:00:52Z", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Separator_Time));

			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringUtc());
			Assert.Equal("2020-06-05T13", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03Z", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03+00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03+00:00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T13:00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00Z", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00+00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00+00:00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T13:00:52", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00:52Z", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00:52+00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00:52+00:00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));

			Assert.Equal("2020-06-05T13:00:52.012", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All));
			Assert.Equal("2020-06-05T03:00:52.012Z", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Utc));
			Assert.Equal("2020-06-05T03:00:52.012+00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_Hour));
			Assert.Equal("2020-06-05T03:00:52.012+00:00", dt.ToIso8601StringUtc(Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondMillis | Iso8601Parts.Separator_All | Iso8601Parts.Tz_HourMinute));
		}
		[Fact]
		public void CtorAndDateParts()
		{
			UtcDateTime dt = new UtcDateTime(2020, 7, 14, 16, 24, 59, 129);
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
			UtcDateTime dt = new UtcDateTime(2020, 7, 14, 16, 24, 59, 129);
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
			Random rng = new Random();
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

						UtcDateTime dt = new UtcDateTime(year, month, day, hour, minute, second, millis);
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
			UtcDateTime dt = new UtcDateTime(1999, 1, 1);
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
			UtcDateTime dt = new UtcDateTime(2004, 2, 29);
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
			UtcDateTime dt = new UtcDateTime(2000, 1, 31);
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
			UtcDateTime dt = new UtcDateTime(2000, 1, 31);
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
