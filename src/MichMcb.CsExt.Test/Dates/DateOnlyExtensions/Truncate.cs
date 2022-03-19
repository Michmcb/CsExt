namespace MichMcb.CsExt.Test.Dates.DateOnlyExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public static class Truncate
	{
		[Fact]
		public static void TruncatingDateOnly()
		{
			Assert.Equal(new DateOnly(2000, 1, 1), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Year));
			Assert.Equal(new DateOnly(2000, 5, 1), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Month));
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Day));
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Hour));
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Minute));
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Second));
			Assert.Equal(new DateOnly(2000, 5, 20), new DateOnly(2000, 5, 20).Truncate(DateTimePart.Millisecond));
			Assert.Throws<ArgumentOutOfRangeException>(() => new DateOnly(2000, 5, 20).Truncate((DateTimePart)9999));
		}
	}
}
