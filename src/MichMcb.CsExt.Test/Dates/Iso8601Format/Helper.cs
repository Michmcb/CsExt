namespace MichMcb.CsExt.Test.Dates.Iso8601Format
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class Helper
	{
		public static void CheckValid(int expectedLength, Iso8601Parts format)
		{
			Iso8601Format fmt = Iso8601Format.TryCreate(format).ValueOrException();
			Assert.Equal(expectedLength, fmt.LengthRequired);
			Assert.Equal(format, fmt.Format);
		}
		public static void CheckInvalid(string errorMsg, Iso8601Parts format)
		{
			string? err = Iso8601Format.TryCreate(format).ErrorOr(null);
			Assert.Equal(err, errorMsg);
		}
	}
}
