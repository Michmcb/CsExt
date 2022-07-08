namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using Xunit;
	public static class TryCreate
	{
		[Fact]
		public static void CorrectLengths()
		{
			// None, i.e. default, should be the same as Format_ExtendedFormat_UtcTz
			//Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sssZ".Length, Iso8601Parts.None);
			//Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sssZ".Length, default);

			// Built-in ones
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sssZ".Length, Iso8601Parts.Format_ExtendedFormat_UtcTz);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss+00:00".Length, Iso8601Parts.Format_ExtendedFormat_FullTz);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss".Length, Iso8601Parts.Format_ExtendedFormat_LocalTz);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ssZ".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_UtcTz);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss+00:00".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_FullTz);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss".Length, Iso8601Parts.Format_ExtendedFormat_NoFractional_LocalTz);
			Helper.CheckValid("yyyyMMddTHHmmss.sssZ".Length, Iso8601Parts.Format_BasicFormat_UtcTz);
			Helper.CheckValid("yyyyMMddTHHmmss.sss+0000".Length, Iso8601Parts.Format_BasicFormat_FullTz);
			Helper.CheckValid("yyyyMMddTHHmmss.sss".Length, Iso8601Parts.Format_BasicFormat_LocalTz);
			Helper.CheckValid("yyyyMMddTHHmmssZ".Length, Iso8601Parts.Format_BasicFormat_NoFractional_UtcTz);
			Helper.CheckValid("yyyyMMddTHHmmss+0000".Length, Iso8601Parts.Format_BasicFormat_NoFractional_FullTz);
			Helper.CheckValid("yyyyMMddTHHmmss".Length, Iso8601Parts.Format_BasicFormat_NoFractional_LocalTz);
			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Format_DateOnly);
			Helper.CheckValid("yyyyMMdd".Length, Iso8601Parts.Format_DateOnlyWithoutSeparators);
			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Format_DateOrdinal);

			// Custom ones
			Helper.CheckValid("yyyy-MM".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonth);

			Helper.CheckValid("yyyy-MM-DDT00+HH".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour | Iso8601Parts.Separator_Date | Iso8601Parts.Tz_Hour);

			Helper.CheckValid("yyyyddd".Length, Iso8601Parts.YearDay);
			Helper.CheckValid("yyyydddTHH".Length, Iso8601Parts.YearDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyydddTHHmm".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyydddTHHmmss".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyydddTHHmmss.sss".Length, Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay);
			Helper.CheckValid("yyyy-dddTHH".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyy-dddTHHmm".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyy-dddTHHmmss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyy-dddTHHmmss.sss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("yyyy-ddd".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay);
			Helper.CheckValid("yyyy-dddTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyy-dddTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyy-dddTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyy-dddTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("yyyyMMdd".Length, Iso8601Parts.YearMonthDay);
			Helper.CheckValid("yyyyMMddTHH".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyyMMddTHHmm".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyyMMddTHHmmss".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyyMMddTHHmmss.sss".Length, Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay);
			Helper.CheckValid("yyyy-MM-ddTHH".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyy-MM-ddTHHmm".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyy-MM-ddTHHmmss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyy-MM-ddTHHmmss.sss".Length, Iso8601Parts.Separator_Date | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("yyyy-MM-dd".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay);
			Helper.CheckValid("yyyy-MM-ddTHH".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.Hour);
			Helper.CheckValid("yyyy-MM-ddTHH:mm".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinute);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("yyyy-MM-ddTHH:mm:ss.sss".Length, Iso8601Parts.Separator_All | Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("THH".Length, Iso8601Parts.Hour);
			Helper.CheckValid("THHmm".Length, Iso8601Parts.HourMinute);
			Helper.CheckValid("THHmmss".Length, Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("THHmmss.sss".Length, Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("THHZ".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THHmmZ".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THHmmssZ".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THHmmss.sssZ".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Utc);

			Helper.CheckValid("THH+00".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THHmm+00".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THHmmss+00".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THHmmss.sss+00".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Hour);

			Helper.CheckValid("THH+0000".Length, Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmm+0000".Length, Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmmss+0000".Length, Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmmss.sss+0000".Length, Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute);

			Helper.CheckValid("THH".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour);
			Helper.CheckValid("THH:mm".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute);
			Helper.CheckValid("THH:mm:ss".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond);
			Helper.CheckValid("THH:mm:ss.sss".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional);

			Helper.CheckValid("THHZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THH:mmZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THH:mm:ssZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc);
			Helper.CheckValid("THH:mm:ss.sssZ".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Utc);

			Helper.CheckValid("THH+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THH:mm+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THH:mm:ss+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Hour);
			Helper.CheckValid("THH:mm:ss.sss+00".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_Hour);

			Helper.CheckValid("THH+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm:ss+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm:ss.sss+0000".Length, Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute);

			Helper.CheckValid("THH+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmm+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmmss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THHmmss.sss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute);

			Helper.CheckValid("THH+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.Hour | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinute | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm:ss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_HourMinute);
			Helper.CheckValid("THH:mm:ss.sss+00:00".Length, Iso8601Parts.Separator_Tz | Iso8601Parts.Separator_Time | Iso8601Parts.HourMinuteSecondFractional | Iso8601Parts.Tz_HourMinute);
		}
		[Fact]
		public static void BadFormats()
		{
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_Hour);
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_HourMinute);
			Helper.CheckInvalid("At least a date and time are required", Iso8601Parts.Tz_Utc);
			
			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_Hour);
			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_HourMinute);
			Helper.CheckInvalid("If a timezone is specified, a time is required", Iso8601Parts.YearMonthDay | Iso8601Parts.Tz_Utc);
			
			Helper.CheckInvalid("Writing year and month only without separators is not valid; it can be confused with yyMMdd", Iso8601Parts.YearMonth);
			
			Helper.CheckInvalid("The provided format for the date portion needs to specify more than just a year", Iso8601Parts.Year);
			
			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.Week | Iso8601Parts.Day);
			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.Week | Iso8601Parts.Month);
			Helper.CheckInvalid("The provided format for the date portion is not valid", Iso8601Parts.MonthDay);
			
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute | Iso8601Parts.Second | Iso8601Parts.Fractional);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute | Iso8601Parts.Second);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Minute);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Second | Iso8601Parts.Fractional);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Second);
			Helper.CheckInvalid("The provided format for the time portion is not valid", Iso8601Parts.Fractional);
			
			Helper.CheckInvalid("Timezone designator can't be just minutes; it needs hours and minutes", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Minute);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_Hour);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_Minute);
			Helper.CheckInvalid("The provided format for the timezone designator is not valid", Iso8601Parts.YearMonthDay | Iso8601Parts.HourMinuteSecond | Iso8601Parts.Tz_Utc | Iso8601Parts.Tz_HourMinute);
		}

	}
}
