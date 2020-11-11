namespace MichMcb.CsExt
{
	using System;
	/// <summary>
	/// Specifies different parts of an ISO-8601 string.
	/// </summary>
	[Flags]
	public enum Iso8601Parts
	{
		/// <summary>
		/// Nothing.
		/// </summary>
		None = 0,
		/// <summary>
		/// A good default format: "yyyy-MM-ddTHH:mm:ss.sssZ"
		/// This is known in ISO-8601 as "Extended Format"
		/// </summary>
		Format_ExtendedFormat_UtcTz = YearMonthDay | HourMinuteSecondMillis | Tz_Utc | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss+00:00
		/// </summary>
		Format_ExtendedFormat_FullTz = YearMonthDay | HourMinuteSecondMillis | Tz_HourMinute | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss
		/// </summary>
		Format_ExtendedFormat_LocalTz = YearMonthDay | HourMinuteSecondMillis | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ssZ
		/// </summary>
		Format_ExtendedFormat_NoMillis_UtcTz = YearMonthDay | HourMinuteSecond | Tz_Utc | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss+00:00
		/// </summary>
		Format_ExtendedFormat_NoMillis_FullTz = YearMonthDay | HourMinuteSecond | Tz_HourMinute | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss
		/// </summary>
		Format_ExtendedFormat_NoMillis_LocalTz = YearMonthDay | HourMinuteSecond | Separator_All,

		/// <summary>
		/// Everything, except without separators: "yyyyMMddTHHmmss.sssZ"
		/// This is known in ISO-8601 as "Basic Format"
		/// </summary>
		Format_BasicFormat_UtcTz = YearMonthDay | HourMinuteSecondMillis | Tz_Utc,
		/// <summary>
		/// yyyyMMddTHHmmss.sss+00:00
		/// </summary>
		Format_BasicFormat_FullTz = YearMonthDay | HourMinuteSecondMillis | Tz_HourMinute,
		/// <summary>
		/// yyyyMMddTHHmmss.sss
		/// </summary>
		Format_BasicFormat_LocalTz = YearMonthDay | HourMinuteSecondMillis,
		/// <summary>
		/// yyyyMMddTHHmmssZ
		/// </summary>
		Format_BasicFormat_NoMillis_UtcTz = YearMonthDay | HourMinuteSecond | Tz_Utc,
		/// <summary>
		/// yyyyMMddTHHmmss+0000
		/// </summary>
		Format_BasicFormat_NoMillis_FullTz = YearMonthDay | HourMinuteSecond | Tz_HourMinute,
		/// <summary>
		/// yyyyMMddTHHmmss+0000
		/// </summary>
		Format_BasicFormat_NoMillis_LocalTz = YearMonthDay | HourMinuteSecond,

		/// <summary>
		/// yyyy-MM-dd
		/// </summary>
		Format_DateOnly = YearMonthDay | Separator_Date,
		/// <summary>
		/// yyyyMMdd
		/// </summary>
		Format_DateOnlyWithoutSeparators = YearMonthDay,
		/// <summary>
		/// yyyy-ddd
		/// </summary>
		Format_DateOrdinal = YearDay | Separator_Date,
		/// <summary>
		/// --MM-dd
		/// </summary>
		Format_VcfUnknownYear = MonthDay | Separator_Date,

		/// <summary>
		/// A mask to get only Separator-specific parts
		/// </summary>
		Mask_Separators =	0b0000_0000_0000_0111,
		/// <summary>
		/// A mask to get only Date-specific parts
		/// </summary>
		Mask_Date =			0b0000_0000_1101_0000,
		/// <summary>
		/// A mask to get only Time-specific parts
		/// </summary>
		Mask_Time =			0b0000_1111_0000_0000,
		/// <summary>
		/// A mask to get only Timezone designator specific parts
		/// </summary>
		Mask_Tz =			0b0111_0000_0000_0000,

		Separator_All = Separator_Date | Separator_Time | Separator_Tz,
		Separator_Date = 0b0000_0000_0000_0100,
		Separator_Time = 0b0000_0000_0000_0010,
		Separator_Tz = 0b0000_0000_0000_0001,
		Separator_DateAndTime = Separator_Date | Separator_Time,
		Separator_DateAndTz = Separator_Date | Separator_Tz,
		Separator_TimeAndTz = Separator_Time | Separator_Tz,

		Year = 0b0000_0000_1000_0000,
		Month = 0b0000_0000_0100_0000,
		// When you uncomment this, set Mask_Date so it includes this: Week = 0b0000_0000_0010_0000,
		Day = 0b0000_0000_0001_0000,
		YearMonthDay = Year | Month | Day,
		/// <summary>
		/// Year and month only. This must be combined with at least Separator_Date (because yyyyMM can be confused with yyMMdd)
		/// </summary>
		YearMonth = Year | Month,
		MonthDay = Month | Day,
		/// <summary>
		/// Year and Ordinal days (i.e. 1~366)
		/// </summary>
		YearDay = Year | Day,
		//YearWeek = Year | Week,
		//YearWeekWeekday = Year | Week | Day,

		Millis = 0b0000_0001_0000_0000,
		Second = 0b0000_0010_0000_0000,
		Minute = 0b0000_0100_0000_0000,
		Hour = 0b0000_1000_0000_0000,
		HourMinuteSecondMillis = Hour | Minute | Second | Millis,
		HourMinuteSecond = Hour | Minute | Second,
		HourMinute = Hour | Minute,

		/// <summary>
		/// Timezone designator will be "Z", indicating UTC
		/// </summary>
		Tz_Utc = 0b0001_0000_0000_0000,
		/// <summary>
		/// Timezone designator will be +/- HH
		/// </summary>
		Tz_Hour = 0b0100_0000_0000_0000,
		Tz_Minute = 0b0010_0000_0000_0000,
		/// <summary>
		/// Timezone designator will be +/- HH:mm
		/// </summary>
		Tz_HourMinute = Tz_Hour | Tz_Minute
	}
}
