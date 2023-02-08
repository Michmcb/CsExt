namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;

	[SimpleJob(RuntimeMoniker.Net70)]
	[SimpleJob(RuntimeMoniker.Net60)]
	[RPlotExporter]
	public class ParseInt
	{
		[Benchmark]
		public bool DotNet()
		{
			return int.TryParse("123456789", System.Globalization.NumberStyles.None, null, out int r);
		}
		[Benchmark]
		public Maybe<int, string> LatinInt32()
		{
			return Parse.LatinInt("123456789");
		}
	}
}
