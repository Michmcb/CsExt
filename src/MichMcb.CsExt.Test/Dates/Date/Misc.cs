namespace MichMcb.CsExt.Test.Dates.Date
{
	using MichMcb.CsExt.Dates;
	using Xunit;

	public static class Misc
	{
		[Fact]
		public static void Truncate()
		{
			Date d = new(2021, 5, 20);
			Assert.Equal(new Date(2021, 5, 20), d.Truncate(DateTimePart.Millisecond));
			Assert.Equal(new Date(2021, 5, 20), d.Truncate(DateTimePart.Second));
			Assert.Equal(new Date(2021, 5, 20), d.Truncate(DateTimePart.Minute));
			Assert.Equal(new Date(2021, 5, 20), d.Truncate(DateTimePart.Hour));
			Assert.Equal(new Date(2021, 5, 20), d.Truncate(DateTimePart.Day));
			Assert.Equal(new Date(2021, 5, 1), d.Truncate(DateTimePart.Month));
			Assert.Equal(new Date(2021, 1, 1), d.Truncate(DateTimePart.Year));
		}
	}
}
