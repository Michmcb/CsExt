namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class Helper
	{
		public static void CheckValid(int expectedLength, Iso8601Parts format, int decimalPlaces)
		{
			Iso8601Format fmt = Iso8601Format.TryCreate(format, decimalPlaces).ValueOrException();
			Assert.Equal(expectedLength, fmt.LengthRequired);
			Assert.Equal(format, fmt.Format);
			bool hasFractionalFlag = (fmt.Format & Iso8601Parts.Fractional) == Iso8601Parts.Fractional;
			Assert.Equal(decimalPlaces != 0, hasFractionalFlag);
		}
		public static void CheckInvalid(string errorMsg, Iso8601Parts format, int decimalPlaces)
		{
			string? err = Iso8601Format.TryCreate(format, decimalPlaces).ErrorOr(null);
			Assert.Equal(err, errorMsg);
		}
	}
}
