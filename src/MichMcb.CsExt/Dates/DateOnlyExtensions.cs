#if NET6_0
namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// Extensions methods for <see cref="DateOnly"/>.
	/// </summary>
	public static class DateOnlyExtensions
	{
		/// <summary>
		/// Calculates a year/month/day given <paramref name="d"/>.
		/// </summary>
		public static void Deconstruct(this DateOnly d, out int year, out int month, out int day)
		{
			UtcDateTime.DatePartsFromTotalDays(d.DayNumber, out year, out month, out day);
		}
	}
}
#endif