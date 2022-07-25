namespace MichMcb.CsExt.Test.Dates.Tz
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class TryCreate
	{
		[Fact]
		public static void Valid()
		{
			Tz tz = Tz.TryCreate(10, 30).ValueOrException();
			Assert.Equal(10, tz.Hours);
			Assert.Equal(30, tz.Minutes);

			tz = Tz.TryCreate(-10, 30).ValueOrException();
			Assert.Equal(-10, tz.Hours);
			Assert.Equal(-30, tz.Minutes);
		}
	}
}
