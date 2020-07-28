namespace MichMcb.CsExt.Test
{
	using static MichMcb.CsExt.Functions;
	using Xunit;
	using System;
	public class OptTest
	{
		[Fact]
		public void Opt()
		{
			Opt<int> x = Some(30);
			Assert.True(x.Ok);
			Assert.True(x);
			Assert.Equal(30, x.Value);

			x = None<int>();
			Assert.False(x.Ok);
			Assert.False(x);
			Assert.Equal(default, x.Value);

		}
		[Fact]
		public void Ex()
		{
			int d = 0;
			Ex<DivideByZeroException> ex = Try<DivideByZeroException>(() => { int x = 1 / d; x += 1; });
			TryCatch<DivideByZeroException>(() => { int x = 1 / d; x += 1; }, e => Console.WriteLine(e.ToString()));
		}
	}
}
