﻿namespace MichMcb.CsExt.Dates
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
		/// yyyy-MM-ddTHH:mm:ss.sssZ
		/// A good default format.
		/// This is known in ISO-8601 as "Extended Format"
		/// </summary>
		Format_ExtendedFormat_UtcTz = YearMonthDay | HourMinuteSecondFractional | Tz_Utc | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss+00:00
		/// </summary>
		Format_ExtendedFormat_FullTz = YearMonthDay | HourMinuteSecondFractional | Tz_HourMinute | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss.sss
		/// </summary>
		Format_ExtendedFormat_LocalTz = YearMonthDay | HourMinuteSecondFractional | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ssZ
		/// </summary>
		Format_ExtendedFormat_NoFractional_UtcTz = YearMonthDay | HourMinuteSecond | Tz_Utc | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss+00:00
		/// </summary>
		Format_ExtendedFormat_NoFractional_FullTz = YearMonthDay | HourMinuteSecond | Tz_HourMinute | Separator_All,
		/// <summary>
		/// yyyy-MM-ddTHH:mm:ss
		/// </summary>
		Format_ExtendedFormat_NoFractional_LocalTz = YearMonthDay | HourMinuteSecond | Separator_All,

		/// <summary>
		/// yyyyMMddTHHmmss.sssZ
		/// Everything, except without separators
		/// This is known in ISO-8601 as "Basic Format"
		/// </summary>
		Format_BasicFormat_UtcTz = YearMonthDay | HourMinuteSecondFractional | Tz_Utc,
		/// <summary>
		/// yyyyMMddTHHmmss.sss+0000
		/// </summary>
		Format_BasicFormat_FullTz = YearMonthDay | HourMinuteSecondFractional | Tz_HourMinute,
		/// <summary>
		/// yyyyMMddTHHmmss.sss
		/// </summary>
		Format_BasicFormat_LocalTz = YearMonthDay | HourMinuteSecondFractional,
		/// <summary>
		/// yyyyMMddTHHmmssZ
		/// </summary>
		Format_BasicFormat_NoFractional_UtcTz = YearMonthDay | HourMinuteSecond | Tz_Utc,
		/// <summary>
		/// yyyyMMddTHHmmss+0000
		/// </summary>
		Format_BasicFormat_NoFractional_FullTz = YearMonthDay | HourMinuteSecond | Tz_HourMinute,
		/// <summary>
		/// yyyyMMddTHHmmss
		/// </summary>
		Format_BasicFormat_NoFractional_LocalTz = YearMonthDay | HourMinuteSecond,

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
		/// A mask to get only Separator-specific parts
		/// </summary>
		Mask_Separators =	0b0000_0000_0000_0111,
		/// <summary>
		/// A mask to get only Date-specific parts
		/// </summary>
		Mask_Date =			0b0000_0000_1111_0000,
		/// <summary>
		/// A mask to get only Time-specific parts
		/// </summary>
		Mask_Time =			0b0000_1111_0000_0000,
		/// <summary>
		/// A mask to get only Timezone designator specific parts
		/// </summary>
		Mask_Tz =			0b0111_0000_0000_0000,

		/// <summary>
		/// Separators for Date, Time, and Timezone
		/// </summary>
		Separator_All = Separator_Date | Separator_Time | Separator_Tz,
		/// <summary>
		/// Separators for year/month/day
		/// </summary>
		Separator_Date = 0b0000_0000_0000_0100,
		/// <summary>
		/// Separators for hour/minute/second (milliseconds always preceded with a dot, as that is required; it isn't an optional separator)
		/// </summary>
		Separator_Time = 0b0000_0000_0000_0010,
		/// <summary>
		/// Separator for timezone when hour/minute is used
		/// </summary>
		Separator_Tz = 0b0000_0000_0000_0001,

		/// <summary>
		/// Year
		/// </summary>
		Year = 0b0000_0000_1000_0000,
		/// <summary>
		/// Month
		/// </summary>
		Month = 0b0000_0000_0100_0000,
		/// <summary>
		/// Week
		/// </summary>
		Week = 0b0000_0000_0010_0000,
		/// <summary>
		/// Day
		/// </summary>
		Day = 0b0000_0000_0001_0000,
		/// <summary>
		/// Year, Month, and Day
		/// </summary>
		YearMonthDay = Year | Month | Day,
		/// <summary>
		/// Year and month only. This must be combined with Separator_Date (because yyyyMM can be confused with yyMMdd)
		/// </summary>
		YearMonth = Year | Month,
		/// <summary>
		/// Month and day only. When omitting year, it results in a format like --MMdd or --MM-dd
		/// </summary>
		MonthDay = Month | Day,
		/// <summary>
		/// Year and ordinal days (i.e. 1 to 366)
		/// </summary>
		YearDay = Year | Day,
		/// <summary>
		/// Year and week
		/// </summary>
		YearWeek = Year | Week,
		/// <summary>
		/// Year, week, and day of week
		/// </summary>
		YearWeekDay = Year | Week | Day,

		/// <summary>
		/// Fractional
		/// </summary>
		Fractional = 0b0000_0001_0000_0000,
		/// <summary>
		/// Second
		/// </summary>
		Second = 0b0000_0010_0000_0000,
		/// <summary>
		/// Minute
		/// </summary>
		Minute = 0b0000_0100_0000_0000,
		/// <summary>
		/// Hour
		/// </summary>
		Hour = 0b0000_1000_0000_0000,
		/// <summary>
		/// Hour, Minute, Second, and Fractional
		/// </summary>
		HourMinuteSecondFractional = Hour | Minute | Second | Fractional,
		/// <summary>
		/// Hour, Minute, Second
		/// </summary>
		HourMinuteSecond = Hour | Minute | Second,
		/// <summary>
		/// Hour, Minute
		/// </summary>
		HourMinute = Hour | Minute,

		/// <summary>
		/// Timezone designator will be "Z", indicating UTC
		/// </summary>
		Tz_Utc = 0b0001_0000_0000_0000,
		/// <summary>
		/// Timezone designator will include hours
		/// </summary>
		Tz_Hour = 0b0100_0000_0000_0000,
		/// <summary>
		/// Timezone designator will include minutes; can only appear alongside <see cref="Tz_Hour"/>
		/// </summary>
		Tz_Minute = 0b0010_0000_0000_0000,
		/// <summary>
		/// Timezone designator will be +/- HH:mm
		/// </summary>
		Tz_HourMinute = Tz_Hour | Tz_Minute
	}
}
