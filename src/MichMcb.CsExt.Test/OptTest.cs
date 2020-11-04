namespace MichMcb.CsExt.Test
{
	using static MichMcb.CsExt.Functions;
	using Xunit;
	public class OptTest
	{
		[Fact]
		public void Opt()
		{
			Opt<int> x = Some(30);
			Assert.True(x.Ok);
			Assert.Equal(30, x.Val);

			x = None<int>();
			Assert.False(x.Ok);
			Assert.Equal(default, x.Val);
		}
	}
}
