namespace MichMcb.CsExt.Test.Dates.DateTimeExtensions
{
	using MichMcb.CsExt.Dates;
	using System;
	using Xunit;

	public sealed class Truncate
	{
		[Fact]
		public void TruncatingDateTime()
		{
			DateTime dt = new(2020, 7, 14, 16, 24, 59, 129);
			Assert.Equal(dt, dt.Truncate(DateTimePart.Millisecond));
			Assert.Equal(new DateTime(2020, 7, 14, 16, 24, 59), dt.Truncate(DateTimePart.Second));
			Assert.Equal(new DateTime(2020, 7, 14, 16, 24, 0), dt.Truncate(DateTimePart.Minute));
			Assert.Equal(new DateTime(2020, 7, 14, 16, 0, 0), dt.Truncate(DateTimePart.Hour));
			Assert.Equal(new DateTime(2020, 7, 14, 0, 0, 0), dt.Truncate(DateTimePart.Day));
			Assert.Equal(new DateTime(2020, 7, 1, 0, 0, 0), dt.Truncate(DateTimePart.Month));
			Assert.Equal(new DateTime(2020, 1, 1, 0, 0, 0), dt.Truncate(DateTimePart.Year));
		}
	}
}
