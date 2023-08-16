namespace MichMcb.CsExt.Dates
{
	using System;

	/// <summary>
	/// Represents the date component of an ISO-8601 string.
	/// </summary>
	public readonly struct Iso8601DatePart
	{
		private Iso8601DatePart(int totalDays, Iso8601DatePartType type)
		{
			TotalDays = totalDays;
			Type = type;
		}
		/// <summary>
		/// The type of this date component.
		/// </summary>
		public Iso8601DatePartType Type { get; }
		/// <summary>
		/// The total days this component represents.
		/// </summary>
		public int TotalDays { get; }
#if NET6_0_OR_GREATER
		/// <summary>
		/// Equivalent to calling <see cref="DateOnly.FromDayNumber(int)"/> with <see cref="TotalDays"/>.
		/// </summary>
		/// <returns>A <see cref="DateOnly"/>.</returns>
		public DateOnly AsDateOnly()
		{
			return DateOnly.FromDayNumber(TotalDays);
		}
#endif
		/// <summary>
		/// Creates a new <see cref="DateTime"/> at midnight, with the provided <paramref name="kind"/>.
		/// </summary>
		/// <param name="kind">The kind the newly created <see cref="DateTime"/> will have.</param>
		/// <returns>A <see cref="DateTime"/>.</returns>
		public DateTime AsDate(DateTimeKind kind)
		{
			return new DateTime(AsTimeSpan().Ticks, kind);
		}
		/// <summary>
		/// Equivalent to calling <see cref="TimeSpan.FromDays(double)"/> with <see cref="TotalDays"/>.
		/// </summary>
		/// <returns>A <see cref="TimeSpan"/>.</returns>
		public TimeSpan AsTimeSpan()
		{
			return new TimeSpan(TotalDays, 0, 0, 0);
		}
		/// <summary>
		/// Equivalent to calling <see cref="IsoYearWeek.Create(DateTime)"/> with the result of <see cref="AsDate(DateTimeKind)"/>.
		/// </summary>
		/// <returns>An <see cref="IsoYearWeek"/>.</returns>
		public IsoYearWeek AsIsoYearWeek()
		{
			return IsoYearWeek.Create(AsDate(DateTimeKind.Unspecified));
		}
		/// <summary>
		/// Attempts to creates a new instance of type <see cref="Iso8601DatePartType.YearMonthDay"/>.
		/// </summary>
		/// <returns>An <see cref="Iso8601DatePart"/> on success, or an error message on failure.</returns>
		public static Maybe<Iso8601DatePart, string> TryYearMonthDay(int year, int month, int day)
		{
			return UtcDateTime.TotalDaysFromYearMonthDay(year, month, day).Success(out int totalDays, out string err)
				? new Iso8601DatePart(totalDays, Iso8601DatePartType.YearMonthDay)
				: err;
		}
		/// <summary>
		/// Attempts to creates a new instance of type <see cref="Iso8601DatePartType.YearOrdinalDay"/>.
		/// </summary>
		/// <returns>An <see cref="Iso8601DatePart"/> on success, or an error message on failure.</returns>
		public static Maybe<Iso8601DatePart, string> TryYearOrdinalDay(int year, int day)
		{
			return UtcDateTime.TotalDaysFromYearOrdinalDays(year, day).Success(out int totalDays, out string err)
				? new Iso8601DatePart(totalDays, Iso8601DatePartType.YearOrdinalDay)
				: err;
		}
		/// <summary>
		/// Attempts to creates a new instance of type <see cref="Iso8601DatePartType.YearWeekDay"/>.
		/// </summary>
		/// <returns>An <see cref="Iso8601DatePart"/> on success, or an error message on failure.</returns>
		public static Maybe<Iso8601DatePart, string> TryYearWeekDay(int year, int week, IsoDayOfWeek day)
		{
			return UtcDateTime.TotalDaysFromYearWeekDay(year, week, day).Success(out int totalDays, out string err)
				? new Iso8601DatePart(totalDays, Iso8601DatePartType.YearWeekDay)
				: err;
		}
	}
}