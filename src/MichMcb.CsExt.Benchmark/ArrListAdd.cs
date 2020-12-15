namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using MichMcb.CsExt.Collections;
	using System.Collections.Generic;

	[SimpleJob(RuntimeMoniker.NetCoreApp50)]
	[RPlotExporter]
	public class ArrListAdd
	{
		// Pretty much equal
		private readonly List<int> list = new(10);
		private readonly Arr<int> arr = new(10);
		[Benchmark]
		public void List()
		{
			for (int i = 0; i < 10; i++)
			{
				list.Add(i);
			}
			list.Clear();
		}
		[Benchmark]
		public void Arr()
		{
			for (int i = 0; i < 10; i++)
			{
				arr.Add(i);
			}
			arr.Clear();
		}
	}
	[SimpleJob(RuntimeMoniker.NetCoreApp50)]
	[RPlotExporter]
	public class ArrListIterate
	{
		private readonly List<int> list = new(10) { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
		private readonly Arr<int> arr = new(10) { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
		[Benchmark]
		public void List()
		{
			foreach (int item in list)
			{

			}
		}
		[Benchmark]
		public void Arr()
		{
			foreach (int item in arr)
			{

			}
		}
	}
}
