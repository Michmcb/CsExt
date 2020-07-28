using System;

namespace MichMcb.ConsoleExt
{
	/// <summary>
	/// Writes coloured strings to the console.
	/// Does not reset the colour after writing the string.
	/// </summary>
	public static class ColorConsole
	{
		[CLSCompliant(false)]
		public static void Write(in ColoredValue<ulong> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<bool> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<char> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<char[]> buffer)
		{
			Console.ForegroundColor = buffer.Colour;
			Console.Write(buffer.Value);
		}
		public static void Write(in ColoredValue<char[]> buffer, int index, int count)
		{
			Console.ForegroundColor = buffer.Colour;
			Console.Write(buffer.Value, index, count);
		}
		public static void Write(in ColoredValue<double> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<long> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<object> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<float> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<string> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<string> format, object arg0)
		{
			Console.ForegroundColor = format.Colour;
			Console.Write(format.Value, arg0);
		}
		public static void Write(in ColoredValue<string> format, object arg0, object arg1)
		{
			Console.ForegroundColor = format.Colour;
			Console.Write(format.Value, arg0, arg1);
		}
		public static void Write(in ColoredValue<string> format, object arg0, object arg1, object arg2)
		{
			Console.ForegroundColor = format.Colour;
			Console.Write(format.Value, arg0, arg1, arg2);
		}
		public static void Write(in ColoredValue<string> format, params object[] arg)
		{
			Console.ForegroundColor = format.Colour;
			Console.Write(format.Value, arg);
		}
		[CLSCompliant(false)]
		public static void Write(in ColoredValue<uint> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<decimal> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void Write(in ColoredValue<int> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.Write(value.Value);
		}
		public static void WriteLine()
		{
			Console.WriteLine();
		}
		[CLSCompliant(false)]
		public static void WriteLine(in ColoredValue<ulong> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<bool> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<char> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<char[]> buffer)
		{
			Console.ForegroundColor = buffer.Colour;
			Console.WriteLine(buffer.Value);
		}
		public static void WriteLine(in ColoredValue<char[]> buffer, int index, int count)
		{
			Console.ForegroundColor = buffer.Colour;
			Console.WriteLine(buffer.Value, index, count);
		}
		public static void WriteLine(in ColoredValue<double> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<long> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<object> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<float> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<string> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<string> format, object arg0)
		{
			Console.ForegroundColor = format.Colour;
			Console.WriteLine(format.Value, arg0);
		}
		public static void WriteLine(in ColoredValue<string> format, object arg0, object arg1)
		{
			Console.ForegroundColor = format.Colour;
			Console.WriteLine(format.Value, arg0, arg1);
		}
		public static void WriteLine(in ColoredValue<string> format, object arg0, object arg1, object arg2)
		{
			Console.ForegroundColor = format.Colour;
			Console.WriteLine(format.Value, arg0, arg1, arg2);
		}
		public static void WriteLine(in ColoredValue<string> format, params object[] arg)
		{
			Console.ForegroundColor = format.Colour;
			Console.WriteLine(format.Value, arg);
		}
		[CLSCompliant(false)]
		public static void WriteLine(in ColoredValue<uint> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<decimal> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		public static void WriteLine(in ColoredValue<int> value)
		{
			Console.ForegroundColor = value.Colour;
			Console.WriteLine(value.Value);
		}
		[CLSCompliant(false)]
		public static void Write(ulong value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(bool value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(char value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(char[] buffer, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(buffer);
		}
		public static void Write(char[] buffer, int index, int count, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(buffer, index, count);
		}
		public static void Write(double value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(long value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(object value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(float value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(string value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(string format, ConsoleColor colour, object arg0)
		{
			Console.ForegroundColor = colour;
			Console.Write(format, arg0);
		}
		public static void Write(string format, ConsoleColor colour, object arg0, object arg1)
		{
			Console.ForegroundColor = colour;
			Console.Write(format, arg0, arg1);
		}
		public static void Write(string format, ConsoleColor colour, object arg0, object arg1, object arg2)
		{
			Console.ForegroundColor = colour;
			Console.Write(format, arg0, arg1, arg2);
		}
		public static void Write(string format, ConsoleColor colour, params object[] arg)
		{
			Console.ForegroundColor = colour;
			Console.Write(format, arg);
		}
		[CLSCompliant(false)]
		public static void Write(uint value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(decimal value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		public static void Write(int value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.Write(value);
		}
		[CLSCompliant(false)]
		public static void WriteLine(ulong value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(bool value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(char value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(char[] buffer, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(buffer);
		}
		public static void WriteLine(char[] buffer, int index, int count, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(buffer, index, count);
		}
		public static void WriteLine(double value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(long value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(object value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(float value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(string value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(string format, ConsoleColor colour, object arg0)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(format, arg0);
		}
		public static void WriteLine(string format, ConsoleColor colour, object arg0, object arg1)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(format, arg0, arg1);
		}
		public static void WriteLine(string format, ConsoleColor colour, object arg0, object arg1, object arg2)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(format, arg0, arg1, arg2);
		}
		public static void WriteLine(string format, ConsoleColor colour, params object[] arg)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(format, arg);
		}
		[CLSCompliant(false)]
		public static void WriteLine(uint value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(decimal value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
		public static void WriteLine(int value, ConsoleColor colour)
		{
			Console.ForegroundColor = colour;
			Console.WriteLine(value);
		}
	}
}
