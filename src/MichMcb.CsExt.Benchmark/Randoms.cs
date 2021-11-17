namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Rng;
	using System;

	[SimpleJob(RuntimeMoniker.Net60)]
	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class Randoms
	{
		private static readonly IntRng intRng = new();
		private static readonly Random random = new();
		[Benchmark]
		public int DotNet()
		{
			return random.Next();
		}
		[Benchmark]
		public int IntRng()
		{
			return intRng.Next();
		}
	}
}
