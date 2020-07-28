using System;

namespace MichMcb.ConsoleExt
{
	public readonly struct ColoredString : IEquatable<ColoredString>
	{
		public ColoredString(string str, ConsoleColor color)
		{
			Str = str;
			Color = color;
		}
		public string Str { get; }
		public ConsoleColor Color { get; }
		public override bool Equals(object? obj)
		{
			return obj is ColoredString @string && Equals(@string);
		}
		public bool Equals(ColoredString other)
		{
			return Str == other.Str &&
				   Color == other.Color;
		}
		public override int GetHashCode()
		{
			return HashCode.Combine(Str, Color);
		}
		public override string ToString()
		{
			return Str;
		}
		public static bool operator ==(ColoredString left, ColoredString right)
		{
			return left.Equals(right);
		}
		public static bool operator !=(ColoredString left, ColoredString right)
		{
			return !(left == right);
		}
	}
}
