namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Dates;

	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class CharOrString
	{
		[Benchmark]
		public string ConcatInt()
		{
			return 500 + "ello world!";
		}
		[Benchmark]
		public string ConcatIntToString()
		{
			return 500.ToString() + "ello world!";
		}
		[Benchmark]
		public string ConcatMany()
		{
			return string.Concat("hello", 125, "world!");
		}
		[Benchmark]
		public string ConcatManyToString()
		{
			return string.Concat("hello", 125.ToString(), "world!");
		}
	}
}
