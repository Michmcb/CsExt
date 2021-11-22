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
		public IsoYearWeek(int year, int week, int weekDay)
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
		/// The ISO day of week. 1 is Monday, and 7 is Sunday
		/// </summary>
		public int WeekDay { get; }
		/// <summary>
		/// Creates a new instance from the specified <paramref name="dateTime"/>.
		/// </summary>
		/// <param name="dateTime">The UtcDateTime.</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(DateTime dateTime)
		{
			dateTime.GetDateParts(out int year, out int month, out int day);
			return Create(year, month, day);
		}
		/// <summary>
		/// Creates a new instance from the specified <paramref name="dateTime"/>.
		/// </summary>
		/// <param name="dateTime">The UtcDateTime.</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(UtcDateTime dateTime)
		{
			dateTime.GetDateParts(out int year, out int month, out int day);
			return Create(year, month, day);
		}
		/// <summary>
		/// Creates a new instance from the specified year, month, and day.
		/// Note that sometimes, the year you pass in will not be the same as the week that comes out.
		/// This is normal, as for example, 2019-12-30 is the 1st week, 1st day of 2020 (i.e. 2020-W01-1).
		/// </summary>
		/// <param name="year">The year</param>
		/// <param name="month">The month</param>
		/// <param name="day">The day</param>
		/// <returns>The ISO Year, Week, and Weekday.</returns>
		public static IsoYearWeek Create(int year, int month, int day)
		{
			// Thanks to https://en.wikipedia.org/wiki/ISO_week_date#Calculating_the_week_number_from_a_month_and_day_of_the_month_or_ordinal_date

			int totalDays = UtcDateTime.TotalDaysFromYear(year);

			int dayOfYear = (DateTime.IsLeapYear(year) ? UtcDateTime.TotalDaysFromStartLeapYearToMonth : UtcDateTime.TotalDaysFromStartYearToMonth)[month - 1] + day;
			int isoDayOfWeek = ((totalDays + dayOfYear - 1) % 7) + 1;
			int w = (10 + dayOfYear - isoDayOfWeek) / 7;
			if (w < 1)
			{
				// If the week number is 0, then that means it's the last week of the previous year
				return new(year - 1, WeeksInYear(year - 1), isoDayOfWeek);
			}
			if (w > WeeksInYear(year))
			{
				// If the week number is larger than the number of weeks in this year, then that means it's the first week of the next year
				return new(year + 1, 1, isoDayOfWeek);
			}
			return new(year, w, isoDayOfWeek);
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
