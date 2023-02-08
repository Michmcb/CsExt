namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using BenchmarkDotNet.Running;
	using System;
	public static class Program
	{
		public static void Main(string[] args)
		{
			//var util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
			//bool b = util.IsDefined(DateTimePart.Day);
			//b = util.IsDefined((DateTimePart)1000);
			//BenchmarkRunner.Run<EnumUtilIsDefined>();
			//BenchmarkRunner.Run<HashSetVsArray>();
			//BenchmarkRunner.Run<ArrListAdd>();
			//BenchmarkRunner.Run<ArrListIterate>();
			BenchmarkRunner.Run<ParseInt>();
			Console.WriteLine("Enter to exit");
			Console.ReadLine();
		}
	}
}
