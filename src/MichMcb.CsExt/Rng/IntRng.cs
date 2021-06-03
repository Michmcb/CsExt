namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Runtime.CompilerServices;
	using System.Buffers.Binary;

	/// <summary>
	/// A random number generator, implemented as a linear congruential generator, that produces <see cref="int"/>.
	/// Faster than <see cref="Random"/>.
	/// </summary>
	public sealed class IntRng
	{
		// Add 2 after we convert to a double, otherwise we'll overflow
		private const double DoubleIntMaxValue = (int.MaxValue * 2d) + 2d;
		private int seed;
		/// <summary>
		/// Creates a new instance. Uses <see cref="Guid.ToByteArray()"/> on a new <see cref="Guid"/> to get the initial seed value.
		/// </summary>
		public IntRng()
		{
			seed = BinaryPrimitives.ReadInt32LittleEndian(Guid.NewGuid().ToByteArray());
		}
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="seed">The initial seed value to use</param>
		public IntRng(int seed)
		{
			this.seed = seed;
		}
		/// <summary>
		/// Returns a random integer.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Next()
		{
			unchecked
			{
				return seed = seed * 1664525 + 1013904223;
			}
		}
		/// <summary>
		/// Returns a random integer that is <paramref name="minValue"/> or larger, and less than <paramref name="maxValue"/>.
		/// </summary>
		/// <param name="minValue">Inclusive min value.</param>
		/// <param name="maxValue">Exclusive max value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Next(int minValue, int maxValue)
		{
			return (int)(((long)maxValue - minValue) * NextDouble()) + minValue;
		}
		/// <summary>
		/// Returns a double that is 0.0 or larger and less than 1.0.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble()
		{
			return (Next() / DoubleIntMaxValue) + 0.5d;
		}
		/// <summary>
		/// Fills <paramref name="span"/> with random numbers. Not thread-safe.
		/// </summary>
		/// <param name="span">The span to fill with random numbers</param>
		public void NextBytes(in Span<byte> span)
		{
			int i;
			// We'll fill the span in 4-byte increments 
			for (i = 0; i < span.Length - 3; i += 4)
			{
				BinaryPrimitives.WriteInt32LittleEndian(span.Slice(i, 4), Next());
			}
			// If there's any left over, then fill the remainder
			if (i != span.Length)
			{
				Span<byte> extra = stackalloc byte[4];
				BinaryPrimitives.WriteInt32LittleEndian(extra, Next());
				int j = 0;
				for (; i < span.Length; i++)
				{
					span[i] = extra[j++];
				}
			}
		}
	}
}
