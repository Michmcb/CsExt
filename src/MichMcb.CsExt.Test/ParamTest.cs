namespace MichMcb.CsExt.Test
{
	using System;
	using Xunit;
	public sealed class ParamTest
	{
		[Fact]
		public void InClosedRange()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InClosedRange(0, 1, 5, "value", "0, 1, 5"));
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InClosedRange(1, 1, 5, "value", "1, 1, 5"));
			Param.InClosedRange(2, 1, 5, "value", "2, 1, 5");
			Param.InClosedRange(3, 1, 5, "value", "3, 1, 5");
			Param.InClosedRange(4, 1, 5, "value", "4, 1, 5");
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InClosedRange(5, 1, 5, "value", "5, 1, 5"));
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InClosedRange(6, 1, 5, "value", "6, 1, 5"));
		}
		[Fact]
		public void InHalfOpenRange()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InHalfOpenRange(0, 1, 5, "value", "0, 1, 5"));
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InHalfOpenRange(1, 1, 5, "value", "1, 1, 5"));
			Param.InHalfOpenRange(2, 1, 5, "value", "2, 1, 5");
			Param.InHalfOpenRange(3, 1, 5, "value", "3, 1, 5");
			Param.InHalfOpenRange(4, 1, 5, "value", "4, 1, 5");
			Param.InHalfOpenRange(5, 1, 5, "value", "5, 1, 5");
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InHalfOpenRange(6, 1, 5, "value", "6, 1, 5"));
		}
		[Fact]
		public void InOpenRange()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InOpenRange(0, 1, 5, "value", "0, 1, 5"));
			Param.InOpenRange(1, 1, 5, "value", "1, 1, 5");
			Param.InOpenRange(2, 1, 5, "value", "2, 1, 5");
			Param.InOpenRange(3, 1, 5, "value", "3, 1, 5");
			Param.InOpenRange(4, 1, 5, "value", "4, 1, 5");
			Param.InOpenRange(5, 1, 5, "value", "5, 1, 5");
			Assert.Throws<ArgumentOutOfRangeException>(() => Param.InOpenRange(6, 1, 5, "value", "6, 1, 5"));
		}
	}
}
