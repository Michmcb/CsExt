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
		//private static readonly LcgRng lcgRng = new();
		private static readonly PcgRng pcgRng = new();
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
		//[Benchmark]
		//public int LcgRng()
		//{
		//	return lcgRng.NextInt32();
		//}
		[Benchmark]
		public int PcgRng()
		{
			return pcgRng.NextInt32();
		}
		//[Benchmark]
		//public double DotNetDouble()
		//{
		//	return random.NextDouble();
		//}
		//[Benchmark]
		//public double IntRngDouble()
		//{
		//	return intRng.NextDouble();
		//}
		//[Benchmark]
		//public double PcgRngDouble()
		//{
		//	return pcgRng.NextDouble();
		//}
		//[Benchmark]
		//public int DotNetRange()
		//{
		//	return random.Next(0, 100);
		//}
		//[Benchmark]
		//public int IntRngRange()
		//{
		//	return intRng.Next(0, 100);
		//}
		//[Benchmark]
		//public int PcgRngRange()
		//{
		//	return pcgRng.NextInt32(0, 100);
		//}
	}
}
