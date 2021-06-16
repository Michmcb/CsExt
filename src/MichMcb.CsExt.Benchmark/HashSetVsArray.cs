namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class HashSetVsArray
	{
		private static readonly char[] invalidCharss = Path.GetInvalidFileNameChars();
		private static readonly HashSet<char> invalidChars = new(Path.GetInvalidFileNameChars());
		private readonly string str = @"This file| has bad chars?!!?";


		[Benchmark]
		public string StripArray()
		{
			Span<char> newStr = stackalloc char[str.Length];
			int i = 0;
			foreach (char c in str)
			{
				if (!invalidCharss.Contains(c))
				{
					newStr[i++] = c;
				}
			}
			return new string(newStr.Slice(0, i));
		}
		[Benchmark]
		public string StripHashSet()
		{
			Span<char> newStr = stackalloc char[str.Length];
			int i = 0;
			foreach (char c in str)
			{
				if (!invalidChars.Contains(c))
				{
					newStr[i++] = c;
				}
			}
			return new string(newStr.Slice(0, i));
		}
	}
}
