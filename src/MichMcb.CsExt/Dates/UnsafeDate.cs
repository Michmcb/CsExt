namespace MichMcb.CsExt.Dates
{
	internal static class UnsafeDate
	{
		internal static int TotalDaysFromYear(int year)
		{
			// Add extra leap year days; a leap year is divisible by 4, but not by 100, unless also divisible by 400.
			--year;
			return (year * 365) + year / 4 - year / 100 + year / 400;
		}
	}
}
