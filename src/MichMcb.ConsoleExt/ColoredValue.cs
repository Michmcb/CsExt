using System;

namespace MichMcb.ConsoleExt
{
#pragma warning disable CA1815 // Override equals and operator equals on value types
	public readonly struct ColoredValue<T>
#pragma warning restore CA1815 // Override equals and operator equals on value types
	{
		public ColoredValue(T val, ConsoleColor colour)
		{
			Value = val;
			Colour = colour;
		}
		public T Value { get; }
		public ConsoleColor Colour { get; }
		public override int GetHashCode()
		{
			return HashCode.Combine(Value);
		}
		public static implicit operator ColoredValue<T>((T value, ConsoleColor colour) p)
		{
			return new ColoredValue<T>(p.value, p.colour);
		}
	}
}