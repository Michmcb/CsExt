namespace MichMcb.CsExt.Dates
{
	/// <summary>
	/// The type of the date component in an <see cref="Iso8601"/>.
	/// </summary>
	public enum Iso8601DatePartType
	{
		/// <summary>
		/// No date component was found.
		/// </summary>
		None,
		/// <summary>
		/// Date component is year/month/day.
		/// </summary>
		YearMonthDay,
		/// <summary>
		/// Date component is year/ordinal days.
		/// </summary>
		YearOrdinalDay,
		/// <summary>
		/// Date component is year/IsoWeek/DayOfWeek
		/// </summary>
		YearWeekDay,
	}
}