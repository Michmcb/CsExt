namespace MichMcb.CsExt.Test.Dates.UtcDateTime
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Truncate
	{
		[Fact]
		public static void Test()
		{
			UtcDateTime dt = new(2020, 7, 14, 16, 24, 59, 129);
			Assert.Equal(dt, dt.Truncate(DateTimePart.Millisecond));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 24, 59), dt.Truncate(DateTimePart.Second));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 24, 0), dt.Truncate(DateTimePart.Minute));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 16, 0, 0), dt.Truncate(DateTimePart.Hour));
			Assert.Equal(new UtcDateTime(2020, 7, 14, 0, 0, 0), dt.Truncate(DateTimePart.Day));
			Assert.Equal(new UtcDateTime(2020, 7, 1, 0, 0, 0), dt.Truncate(DateTimePart.Month));
			Assert.Equal(new UtcDateTime(2020, 1, 1, 0, 0, 0), dt.Truncate(DateTimePart.Year));
			Assert.Throws<ArgumentOutOfRangeException>(() => dt.Truncate((DateTimePart)999));
		}
	}
}
