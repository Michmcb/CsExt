namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Running;
	public static class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<HashSetVsArray>();
		}
	}
}
