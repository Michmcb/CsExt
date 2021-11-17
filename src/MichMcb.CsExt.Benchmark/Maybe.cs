namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;

	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class Maybe
	{
		[Benchmark]
		public string Success()
		{
			if (new Maybe<int, string>(10).Success(out int val, out string err))
			{
				return "1";
			}
			else
			{
				return "0";
			}
		}
		[Benchmark]
		public string Failure()
		{
			if (new Maybe<int, string>("failed").Success(out int val, out string err))
			{
				return "1";
			}
			else
			{
				return "0";
			}
		}
	}
}
