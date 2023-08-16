namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using System;

	[SimpleJob(RuntimeMoniker.Net70)]
	[RPlotExporter]
	public class NewVsToString
	{
		private readonly string s = "Hello World!";

		[Benchmark]
		public new string ToString()
		{
			return s.AsSpan().ToString();
		}
		[Benchmark]
		public string NewString()
		{
			return new string(s.AsSpan());
		}
	}

	[SimpleJob(RuntimeMoniker.Net60)]
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
