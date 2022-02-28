namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// Extensions methods for <see cref="DayOfWeek"/>.
	/// </summary>
	public static class DayOfWeekExtensions
	{
		/// <summary>
		/// Converts a <see cref="System.DayOfWeek"/> into an <see cref="Dates.IsoDayOfWeek"/>.
		/// Under ISO-8601, Monday is the first day of the week, and Sunday is the last day of the week. 1 is Monday, and 7 is Sunday.
		/// </summary>
		/// <param name="dayOfWeek">The day of week to convert.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static IsoDayOfWeek IsoDayOfWeek(this DayOfWeek dayOfWeek)
		{
			return dayOfWeek switch
			{
				System.DayOfWeek.Monday => Dates.IsoDayOfWeek.Monday,
				System.DayOfWeek.Tuesday => Dates.IsoDayOfWeek.Tuesday,
				System.DayOfWeek.Wednesday => Dates.IsoDayOfWeek.Wednesday,
				System.DayOfWeek.Thursday => Dates.IsoDayOfWeek.Thursday,
				System.DayOfWeek.Friday => Dates.IsoDayOfWeek.Friday,
				System.DayOfWeek.Saturday => Dates.IsoDayOfWeek.Saturday,
				System.DayOfWeek.Sunday => Dates.IsoDayOfWeek.Sunday,
				_ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), "Value for DayOfWeek was outside the range of acceptable values. It was: " + (int)dayOfWeek),
			};
		}
		/// <summary>
		/// Converts an <see cref="Dates.IsoDayOfWeek"/> into a <see cref="System.DayOfWeek"/>.
		/// </summary>
		/// <param name="isoDayOfWeek">The day of week to convert.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		public static DayOfWeek DayOfWeek(this IsoDayOfWeek isoDayOfWeek)
		{
			return isoDayOfWeek switch
			{
				Dates.IsoDayOfWeek.Monday => System.DayOfWeek.Monday,
				Dates.IsoDayOfWeek.Tuesday => System.DayOfWeek.Tuesday,
				Dates.IsoDayOfWeek.Wednesday => System.DayOfWeek.Wednesday,
				Dates.IsoDayOfWeek.Thursday => System.DayOfWeek.Thursday,
				Dates.IsoDayOfWeek.Friday => System.DayOfWeek.Friday,
				Dates.IsoDayOfWeek.Saturday => System.DayOfWeek.Saturday,
				Dates.IsoDayOfWeek.Sunday => System.DayOfWeek.Sunday,
				_ => throw new ArgumentOutOfRangeException(nameof(isoDayOfWeek), "Value for IsoDayOfWeek was outside the range of acceptable values. It was: " + (int)isoDayOfWeek),
			};
		}
	}
}
