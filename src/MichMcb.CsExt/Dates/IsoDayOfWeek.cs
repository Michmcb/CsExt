namespace MichMcb.CsExt.Dates
{
	/// <summary>
	/// Specifies the day of the week according to ISO-8601. You can cast this back and forth between <see cref="int"/> when doing calculations for ISO-8601 things.
	/// </summary>
	public enum IsoDayOfWeek : int
	{
		/// <summary>
		/// Monday (1)
		/// </summary>
		Monday = 1,
		/// <summary>
		/// Tuesday (2)
		/// </summary>
		Tuesday = 2,
		/// <summary>
		/// Wednesday (3)
		/// </summary>
		Wednesday = 3,
		/// <summary>
		/// Thursday (4)
		/// </summary>
		Thursday = 4,
		/// <summary>
		/// Friday (5)
		/// </summary>
		Friday = 5,
		/// <summary>
		/// Saturday (6)
		/// </summary>
		Saturday = 6,
		/// <summary>
		/// Sunday (7)
		/// </summary>
		Sunday = 7,
	}
}
