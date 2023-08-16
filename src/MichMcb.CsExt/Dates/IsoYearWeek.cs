namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// Represents the year, week, and weekday of an ISO Week.
	/// </summary>
	public readonly struct IsoYearWeek
	{
		/// <summary>
		/// Creates a new instance
		/// </summary>
		public IsoYearWeek(int year, int week, IsoDayOfWeek weekDay)
		{
			Year = year;
			Week = week;
			WeekDay = weekDay;
		}
		/// <summary>
		/// The year.
		/// </summary>
		public int Year { get; }
		/// <summary>
		/// The week number of the year.
		/// </summary>
		public int Week { get; }
		/// <summary>
		/// The ISO day of week.
		/// </summary>
		public IsoDayOfWeek WeekDay { get; }
#if NET6_0_OR_GREATER
		/// <summary>
		/// Creates a new instance from the specified <paramref name="dateOnly"/>.
		/// </summary>
		/// <param name="dateOnly">The DateOnly.</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(DateOnly dateOnly)
		{
			dateOnly.GetDateParts(out int year, out int month, out int day);
			return CreateUnchecked(year, month, day);
		}
#endif
		/// <summary>
		/// Creates a new instance from the specified <paramref name="dateTime"/>.
		/// </summary>
		/// <param name="dateTime">The DateTime.</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(DateTime dateTime)
		{
			dateTime.GetDateParts(out int year, out int month, out int day);
			return CreateUnchecked(year, month, day);
		}
		/// <summary>
		/// Creates a new instance from the specified <paramref name="utcDateTime"/>.
		/// </summary>
		/// <param name="utcDateTime">The UtcDateTime.</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(UtcDateTime utcDateTime)
		{
			utcDateTime.GetDateParts(out int year, out int month, out int day);
			return CreateUnchecked(year, month, day);
		}
		/// <summary>
		/// Creates a new instance from the specified year, month, and day.
		/// Note that sometimes, the year you pass in will not be the same as the year that comes out.
		/// This is normal, as for example, 2019-12-30 is the 1st week, 1st day of 2020 (i.e. 2020-W01-1).
		/// </summary>
		/// <param name="year">The year</param>
		/// <param name="month">The month</param>
		/// <param name="day">The day</param>
		/// <returns>The ISO Year, Week, and Weekday, or an error message.</returns>
		public static Maybe<IsoYearWeek, string> TryCreate(int year, int month, int day)
		{
			string? e = DateUtil.ValidateDate(year, month, day);
			if (e != null) return e;

			int dayOfYear = (DateTime.IsLeapYear(year) ? UtcDateTime.TotalDaysFromStartLeapYearToMonth : UtcDateTime.TotalDaysFromStartYearToMonth)[month - 1] + day;
			return TryCreateOrdinalDays(year, dayOfYear);
		}
		/// <summary>
		/// Creates a new instance from the specified year and days.
		/// Note that sometimes, the year you pass in will not be the same as the year that comes out.
		/// This is normal, as for example, 2019-12-30 is the 1st week, 1st day of 2020 (i.e. 2020-W01-1).
		/// </summary>
		/// <param name="year">The year</param>
		/// <param name="dayOfYear">The day of the year</param>
		/// <returns>The ISO Year, Week, and Weekday, or an error message.</returns>
		public static Maybe<IsoYearWeek, string> TryCreateOrdinalDays(int year, int dayOfYear)
		{
			string? e = DateUtil.ValidateOrdinalDays(year, dayOfYear);
			if (e != null) return e;

			return CreateUncheckedOrdinalDays(year, dayOfYear);
		}
		internal static IsoYearWeek CreateUnchecked(int year, int month, int day)
		{
			int dayOfYear = (DateTime.IsLeapYear(year) ? UtcDateTime.TotalDaysFromStartLeapYearToMonth : UtcDateTime.TotalDaysFromStartYearToMonth)[month - 1] + day;
			return CreateUncheckedOrdinalDays(year, dayOfYear);
		}
		internal static IsoYearWeek CreateUncheckedOrdinalDays(int year, int dayOfYear)
		{
			int totalDays = UnsafeDate.TotalDaysFromYear(year);
			// Thanks to https://en.wikipedia.org/wiki/ISO_week_date#Calculating_the_week_number_from_a_month_and_day_of_the_month_or_ordinal_date
			int isoDayOfWeek = ((totalDays + dayOfYear - 1) % 7) + 1;
			int w = (10 + dayOfYear - isoDayOfWeek) / 7;

			return w < 1
				// If the week number is 0, then that means it's the last week of the previous year
				? new IsoYearWeek(year - 1, WeeksInYear(year - 1), (IsoDayOfWeek)isoDayOfWeek)
				: w > WeeksInYear(year)
					// If the week number is larger than the number of weeks in this year, then that means it's the first week of the next year
					? new IsoYearWeek(year + 1, 1, (IsoDayOfWeek)isoDayOfWeek)
					// Otherwise our numbers are good
					: new IsoYearWeek(year, w, (IsoDayOfWeek)isoDayOfWeek);
		}
		/// <summary>
		/// Returns the number of weeks in the provided year.
		/// </summary>
		/// <param name="year">The year.</param>
		/// <returns>The number of weeks in the year; either 52 or 53</returns>
		public static int WeeksInYear(int year)
		{
			// Thanks https://en.wikipedia.org/wiki/ISO_week_date#Weeks_per_year
			// If the current year ends thursday, or the previous year ends wednesday, 53 weeks. Otherwise, 52.
			return DayOfWeekOf31stDecember(year) == 4 ? 53 : DayOfWeekOf31stDecember(year - 1) == 3 ? 53 : 52;
		}
		private static int DayOfWeekOf31stDecember(int year)
		{
			return (year + (year / 4) - (year / 100) + (year / 400)) % 7;
		}
	}
}
