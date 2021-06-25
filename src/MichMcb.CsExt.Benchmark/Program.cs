namespace MichMcb.CsExt.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using BenchmarkDotNet.Running;
	using MichMcb.CsExt.Dates;
	using System;
	using System.Collections.Generic;

	public static class Program
	{
		public static void Main(string[] args)
		{
			//var util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
			//bool b = util.IsDefined(DateTimePart.Day);
			//b = util.IsDefined((DateTimePart)1000);
			//BenchmarkRunner.Run<EnumUtilIsDefined>();
			BenchmarkRunner.Run<FormatUtcDateTime>();
			//BenchmarkRunner.Run<HashSetVsArray>();
			//BenchmarkRunner.Run<ArrListAdd>();
			//BenchmarkRunner.Run<ArrListIterate>();
			Console.WriteLine("Enter to exit");
			Console.ReadLine();
		}
	}
	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class EnumUtilIsDefined
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		private static readonly HashSet<DateTimePart> hashset= new((DateTimePart[])Enum.GetValues(typeof(DateTimePart)));
		[Benchmark]
		public void Compiled()
		{
			util.IsDefinedFunc(DateTimePart.Millisecond);
			util.IsDefinedFunc(DateTimePart.Second);
			util.IsDefinedFunc(DateTimePart.Minute);
			util.IsDefinedFunc(DateTimePart.Hour);
			util.IsDefinedFunc(DateTimePart.Day);
			util.IsDefinedFunc(DateTimePart.Month);
			util.IsDefinedFunc(DateTimePart.Year);
		}
		[Benchmark]
		public void HashSet()
		{
			hashset.Contains(DateTimePart.Millisecond);
			hashset.Contains(DateTimePart.Second);
			hashset.Contains(DateTimePart.Minute);
			hashset.Contains(DateTimePart.Hour);
			hashset.Contains(DateTimePart.Day);
			hashset.Contains(DateTimePart.Month);
			hashset.Contains(DateTimePart.Year);
		}
		[Benchmark]
		public void IsDefined()
		{
			Enum.IsDefined(DateTimePart.Millisecond);
			Enum.IsDefined(DateTimePart.Second);
			Enum.IsDefined(DateTimePart.Minute);
			Enum.IsDefined(DateTimePart.Hour);
			Enum.IsDefined(DateTimePart.Day);
			Enum.IsDefined(DateTimePart.Month);
			Enum.IsDefined(DateTimePart.Year);
		}
	}
	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class EnumUtilTestParse
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		private static readonly Dictionary<string, DateTimePart> vals = new(StringComparer.OrdinalIgnoreCase);
		private static readonly StringComparer sc = StringComparer.OrdinalIgnoreCase;
		private static readonly string Val = "Millisecond";
		[Benchmark]
		public DateTimePart Switch()
		{
			switch (Val)
			{
				case "Millisecond": return DateTimePart.Millisecond;
				case "Second": return DateTimePart.Second;
				case "Minute": return DateTimePart.Minute;
				case "Hour": return DateTimePart.Hour;
				case "Day": return DateTimePart.Day;
				case "Month": return DateTimePart.Month;
				case "Year": return DateTimePart.Year;
				default: return 0;
			}
		}
		[Benchmark]
		public DateTimePart Compiled()
		{
			return util.TryParse(Val).ValueOr(DateTimePart.Year);
		}
		[Benchmark]
		public DateTimePart EnumTryParse()
		{
			Enum.TryParse(Val, out DateTimePart p);
			return p;
		}
	}
	[SimpleJob(RuntimeMoniker.Net50)]
	[RPlotExporter]
	public class EnumUtilTestToString
	{
		private static readonly EnumUtil<DateTimePart> util = EnumUtil<DateTimePart>.Compile(string.Empty, StringComparer.OrdinalIgnoreCase).ValueOr(null);
		[Benchmark]
		public void EnumToString()
		{
			DateTimePart.Millisecond.ToString();
			DateTimePart.Second.ToString();
			DateTimePart.Minute.ToString();
			DateTimePart.Hour.ToString();
			DateTimePart.Day.ToString();
			DateTimePart.Month.ToString();
			DateTimePart.Year.ToString();
		}
		[Benchmark]
		public void EnumUtil()
		{
			util.ToStringFunc(DateTimePart.Millisecond);
			util.ToStringFunc(DateTimePart.Second);
			util.ToStringFunc(DateTimePart.Minute);
			util.ToStringFunc(DateTimePart.Hour);
			util.ToStringFunc(DateTimePart.Day);
			util.ToStringFunc(DateTimePart.Month);
			util.ToStringFunc(DateTimePart.Year);
		}
	}
}
