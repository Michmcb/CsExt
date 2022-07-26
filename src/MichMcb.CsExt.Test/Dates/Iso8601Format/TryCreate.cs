namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;
	public static class TryCreate
	{
		[Fact]
		public static void Defaults()
		{
			{
				Iso8601Format fmt = Iso8601Format.TryCreate(Iso8601Parts.None, 0).ValueOrException();
				Assert.Equal("yyyy-MM-ddTHH:mm:ssZ".Length, fmt.LengthRequired);
				Assert.Equal(Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz, fmt.Format);
				bool hasFractionalFlag = (fmt.Format & Iso8601Parts.Fractional) == Iso8601Parts.Fractional;
				Assert.False(hasFractionalFlag);
			}
			{
				Iso8601Format fmt = Iso8601Format.TryCreate(Iso8601Parts.None, 3).ValueOrException();
				Assert.Equal("yyyy-MM-ddTHH:mm:ss.fffZ".Length, fmt.LengthRequired);
				Assert.Equal(Iso8601Parts.Format_ExtendedFormat_UtcTz, fmt.Format);
				bool hasFractionalFlag = (fmt.Format & Iso8601Parts.Fractional) == Iso8601Parts.Fractional;
				Assert.True(hasFractionalFlag);
			}
		}
		[Fact]
		public static void CorrectLengths()
		{
			// Built-in ones
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sssZ".Length, Iso8601Parts.Format_ExtendedFormat_UtcTz, 3);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss+00:00".Length, Iso8601Parts.Format_ExtendedFormat_FullTz, 3);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss".Length, Iso8601Parts.Format_ExtendedFormat_LocalTz, 3);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ssZ".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz, 0);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss+00:00".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz, 0);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz, 0);
			Helper.CheckValid("yyyyMMddTHHmmss.sssZ".Length, Iso8601Parts.Format_BasicFormat_UtcTz, 3);
			Helper.CheckValid("yyyyMMddTHHmmss.sss+0000".Length, Iso8601Parts.Format_BasicFormat_FullTz, 3);
			Helper.CheckValid("yyyyMMddTHHmmss.sss".Length, Iso8601Parts.Format_BasicFormat_LocalTz, 3);
			Helper.CheckValid("yyyyMMddTHHmmssZ".Length, Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz, 0);
			Helper.CheckValid("yyyyMMddTHHmmss+0000".Length, Iso8601Parts.Format_BasicFormat_NoFractional_FullTz, 0);
			Helper.CheckValid("yyyyMMddTHHmmss".Length, Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz, 0);
			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Format_DateOnly, 0);
			Helper.CheckValid("yyyyMMdd".Length, Iso8601Parts.Format_DateOnlyWithoutSeparators, 0);
			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Format_DateOrdinal, 0);

			// Custom ones
			Helper.CheckValid("yyyy-MM".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonth, 0);

			Helper.CheckValid("yyyy-MM-DDT00+HH".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_Date | Iso8601Parts.Tz_Hour, 0);

			Helper.CheckValid("yyyyddd".Length, Iso8601Parts.YearDay, 0);
			Helper.CheckValid("yyyydddTHH".Length, Iso8601Parts.YearDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyydddTHHmm".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyydddTHHmmss".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyydddTHHmmss.sss".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay, 0);
			Helper.CheckValid("yyyy-dddTHH".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-dddTHHmm".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-dddTHHmmss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-dddTHHmmss.sss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay, 0);
			Helper.CheckValid("yyyy-dddTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-dddTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-dddTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-dddTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-Www".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeek, 0);
			Helper.CheckValid("yyyy-WwwTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeek | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-WwwTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeek | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-WwwTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeek | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-WwwTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeek | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-Www-d".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeekDay, 0);
			Helper.CheckValid("yyyy-Www-dTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeekDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-Www-dTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-Www-dTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-Www-dTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyyWww".Length, Iso8601Parts.YearWeek, 0);
			Helper.CheckValid("yyyyWwwTHH".Length, Iso8601Parts.YearWeek | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyyWwwTHHmm".Length, Iso8601Parts.YearWeek | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyyWwwTHHmmss".Length, Iso8601Parts.YearWeek | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyyWwwTHHmmss.sss".Length, Iso8601Parts.YearWeek | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyyWwwd".Length, Iso8601Parts.YearWeekDay, 0);
			Helper.CheckValid("yyyyWwwdTHH".Length, Iso8601Parts.YearWeekDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyyWwwdTHHmm".Length, Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyyWwwdTHHmmss".Length, Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyyWwwdTHHmmss.sss".Length, Iso8601Parts.YearWeekDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyyMMdd".Length, Iso8601Parts.YearMonthDay, 0);
			Helper.CheckValid("yyyyMMddTHH".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyyMMddTHHmm".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyyMMddTHHmmss".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyyMMddTHHmmss.sss".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay, 0);
			Helper.CheckValid("yyyy-MM-ddTHH".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-MM-ddTHHmm".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-MM-ddTHHmmss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-MM-ddTHHmmss.sss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay, 0);
			Helper.CheckValid("yyyy-MM-ddTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour, 0);
			Helper.CheckValid("yyyy-MM-ddTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("THH".Length, Iso8601Parts.Hour, 0);
			Helper.CheckValid("THHmm".Length, Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("THHmmss".Length, Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("THHmmss.sss".Length, Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("THHZ".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THHmmZ".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THHmmssZ".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THHmmss.sssZ".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Utc, 3);

			Helper.CheckValid("THH+00".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THHmm+00".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THHmmss+00".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THHmmss.sss+00".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Hour, 3);

			Helper.CheckValid("THH+0000".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmm+0000".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmmss+0000".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmmss.sss+0000".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute, 3);

			Helper.CheckValid("THH".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour, 0);
			Helper.CheckValid("THH:mm".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute, 0);
			Helper.CheckValid("THH:mm:ss".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond, 0);
			Helper.CheckValid("THH:mm:ss.sss".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional, 3);

			Helper.CheckValid("THHZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THH:mmZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THH:mm:ssZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc, 0);
			Helper.CheckValid("THH:mm:ss.sssZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Utc, 3);

			Helper.CheckValid("THH+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THH:mm+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THH:mm:ss+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Hour, 0);
			Helper.CheckValid("THH:mm:ss.sss+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Hour, 3);

			Helper.CheckValid("THH+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm:ss+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm:ss.sss+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute, 3);

			Helper.CheckValid("THH+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmm+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmmss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THHmmss.sss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute, 3);

			Helper.CheckValid("THH+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm:ss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute, 0);
			Helper.CheckValid("THH:mm:ss.sss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute, 3);
		}
		[Fact]
		public static void WithDecimalPlaces()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Iso8601Format.GetFormat().WithDecimalPlaces(-1));

			foreach (TimeZoneType tz in Enum.GetValues<TimeZoneType>())
			{
				foreach (bool extended in new bool[] { true, false })
				{
					for (int decimalPlaces = 1; decimalPlaces < 10; decimalPlaces++)
					{
						Iso8601Format fmt = Iso8601Format.GetFormat(tz, extended, 0);
						Iso8601Format fmtDec = fmt.WithDecimalPlaces(decimalPlaces);

						// fmt should be missing the flag, and fmtDec should have the flag
						Assert.Equal(Iso8601Parts.None, fmt.Format & Iso8601Parts.Fractional);
						Assert.Equal(Iso8601Parts.Fractional, fmtDec.Format & Iso8601Parts.Fractional);

						// Removing the flag from fmtDec should be the same thing as the other format
						Assert.Equal(fmt.Format, fmtDec.Format & ~Iso8601Parts.Fractional);

						Assert.Equal(0, fmt.DecimalPlaces);
						Assert.Equal(decimalPlaces, fmtDec.DecimalPlaces);
						Assert.Equal(fmt.LengthRequired + 1 + fmtDec.DecimalPlaces, fmtDec.LengthRequired);
					}
					for (int decimalPlaces = 9; decimalPlaces > 0; decimalPlaces--)
					{
						Iso8601Format fmtDec = Iso8601Format.GetFormat(tz, extended, decimalPlaces);
						Iso8601Format fmt = fmtDec.WithDecimalPlaces(0);

						// fmt should be missing the flag, and fmtDec should have the flag
						Assert.Equal(Iso8601Parts.None, fmt.Format & Iso8601Parts.Fractional);
						Assert.Equal(Iso8601Parts.Fractional, fmtDec.Format & Iso8601Parts.Fractional);

						// Removing the flag from fmtDec should be the same thing as the other format
						Assert.Equal(fmt.Format, fmtDec.Format & ~Iso8601Parts.Fractional);

						Assert.Equal(0, fmt.DecimalPlaces);
						Assert.Equal(decimalPlaces, fmtDec.DecimalPlaces);
						Assert.Equal(fmt.LengthRequired + 1 + fmtDec.DecimalPlaces, fmtDec.LengthRequired);
					}
				}
			}
			{
				// 0 and 0 shouldn't change it
				Iso8601Format fmt1 = Iso8601Format.GetFormat(TimeZoneType.Utc, extended: false, 0);
				Iso8601Format fmt2 = fmt1.WithDecimalPlaces(0);
				Assert.Equal(fmt1.Format, fmt2.Format);
				Assert.Equal(fmt1.DecimalPlaces, fmt2.DecimalPlaces);
				Assert.Equal(fmt1.LengthRequired, fmt2.LengthRequired);
			}
			{
				// non-zero and non-zero, formats should be the same but length/decimal should be changed
				Iso8601Format fmt3 = Iso8601Format.GetFormat(TimeZoneType.Local, extended: false, 3);
				Iso8601Format fmt5 = fmt3.WithDecimalPlaces(5);
				Assert.Equal(fmt3.Format, fmt5.Format);
				Assert.NotEqual(fmt3.DecimalPlaces, fmt5.DecimalPlaces);
				Assert.NotEqual(fmt3.LengthRequired, fmt5.LengthRequired);
				Assert.Equal(2, fmt5.LengthRequired - fmt3.LengthRequired);
				Assert.Equal(2, fmt5.DecimalPlaces - fmt3.DecimalPlaces);
			}
		}
		[Fact]
		public static void BadFormats()
		{
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_Hour, 3);
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_HourMinute, 3);
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_Utc, 3);

			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_Hour, 3);
			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_HourMinute, 3);
			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_Utc, 3);

			Helper.CheckInvalid("Writing year and month only without separators is not valid; it can be confused with yyMMdd", Iso8601Parts.YearMonth, 3);

			Helper.CheckInvalid("The provided format for the date portion needs to specify more than just a year", Iso8601Parts.Year, 3);

			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.Week | Iso8601Parts.Day, 3);
			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.Week | Iso8601Parts.Month, 3);
			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.MonthDay, 3);

			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute | Iso8601Parts.Second | Iso8601Parts.Fractional, 3);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute | Iso8601Parts.Second, 3);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute, 3);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Second | Iso8601Parts.Fractional, 3);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Second, 3);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Fractional, 3);

			Helper.CheckInvalid("Timezone designator can't be just minutes; it needs hours and minutes", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Minute, 3);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_Hour, 3);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_Minute, 3);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_HourMinute, 3);

			Helper.CheckInvalid("Decimal places cannot be less than zero", Iso8601Parts.Format_BasicFormat_UtcTz, -1);
			Assert.Throws<ArgumentOutOfRangeException>(() => Iso8601Format.GetFormat(TimeZoneType.Utc, extended: true, decimalPlaces: -1));
		}
	}
}
