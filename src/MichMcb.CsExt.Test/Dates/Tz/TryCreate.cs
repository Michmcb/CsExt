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
		[Fact]
		public static void Invalid()
		{
			Assert.Equal("Timezone hours out of range. Hours: 15 Minutes: 0", Tz.TryCreate(15, 0).ErrorOr(default).Message);
			Assert.Equal("Timezone hours and minutes out of range. Hours: 14 Minutes: 30", Tz.TryCreate(14, 30).ErrorOr(default).Message);
		}
	}
}
