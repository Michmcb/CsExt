#if NET6_0_OR_GREATER
namespace MichMcb.CsExt.Dates
{
	using System;
	/// <summary>
	/// Extensions methods for <see cref="DateOnly"/>.
	/// </summary>
	public static class DateOnlyExtensions
	{
		/// <summary>
		/// Calculates a year/month/day given <paramref name="dateOnly"/>.
		/// Identical to <see cref="Deconstruct(DateOnly, out int, out int, out int)"/>.
		/// </summary>
		public static void GetDateParts(this DateOnly dateOnly, out int year, out int month, out int day)
		{
			year = dateOnly.Year;
			month= dateOnly.Month;
			day = dateOnly.Day;
		}
		/// <summary>
		/// Calculates a year/month/day given <paramref name="dateOnly"/>.
		/// </summary>
		public static void Deconstruct(this DateOnly dateOnly, out int year, out int month, out int day)
		{
			year = dateOnly.Year;
			month = dateOnly.Month;
			day = dateOnly.Day;
		}
		/// <summary>
		/// Writes <paramref name="dateOnly"/> as yyyy-MM-dd or yyyyMMdd.
		/// </summary>
		/// <param name="dateOnly">The <see cref="DateOnly"/>.</param>
		/// <param name="extended">If true, writes as yyyy-MM-dd. If false, writes as yyyyMMdd</param>
		public static string ToIso8601String(this DateOnly dateOnly, bool extended = true)
		{
			return extended
				? string.Create(10, dateOnly, (destination, d) =>
				{
					d.Deconstruct(out int year, out int month, out int day);
					Formatting.Write4Digits((uint)year, destination, 0);
					destination[4] = '-';
					Formatting.Write2Digits((uint)month, destination, 5);
					destination[7] = '-';
					Formatting.Write2Digits((uint)day, destination, 8);
				})
				: string.Create(8, dateOnly, (destination, d) =>
				{
					d.Deconstruct(out int year, out int month, out int day);
					Formatting.Write4Digits((uint)year, destination, 0);
					Formatting.Write2Digits((uint)month, destination, 4);
					Formatting.Write2Digits((uint)day, destination, 6);
				});
		}
		/// <summary>
		/// Returns a truncated instance so that it is only accurate to the part specified by <paramref name="truncateTo"/>.
		/// For example, if <paramref name="truncateTo"/> is Minute, then Seconds and Milliseconds are set to zero.
		/// Truncating days or months will cause them to be truncated to 1.
		/// </summary>
		public static DateOnly Truncate(this DateOnly dateOnly, DateTimePart truncateTo)
		{
			return truncateTo switch
			{
				DateTimePart.Year => new(dateOnly.Year, 1, 1),
				DateTimePart.Month => new(dateOnly.Year, dateOnly.Month, 1),
				DateTimePart.Day => dateOnly,
				DateTimePart.Hour => dateOnly,
				DateTimePart.Minute => dateOnly,
				DateTimePart.Second => dateOnly,
				DateTimePart.Millisecond => dateOnly,
				_ => throw new ArgumentOutOfRangeException(nameof(truncateTo), "Parameter was not a valid value for DateTimePart"),
			};
		}
	}
}
#endif