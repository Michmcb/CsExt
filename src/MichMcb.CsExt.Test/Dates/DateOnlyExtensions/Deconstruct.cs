namespace MichMcb.CsExt.Test.Dates.DateOnlyExtensions
{
	using System;
	using Xunit;
	using MichMcb.CsExt.Dates;

	public static class Deconstruct
	{
		[Fact]
		public static void WorksOk()
		{
			DateOnly max = new(3000, 1, 1);
			DateOnly d = new(2000, 1, 1);
			while (d != max)
			{
				DateOnlyExtensions.Deconstruct(d, out int year, out int month, out int day);
				Assert.Equal(d.Year, year);
				Assert.Equal(d.Month, month);
				Assert.Equal(d.Day, day);
				d = d.AddDays(1);
			}
		}
	}
}
