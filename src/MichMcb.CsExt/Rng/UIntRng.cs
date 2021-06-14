namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Buffers.Binary;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// A random number generator, implemented as a linear congruential generator, that produces <see cref="uint"/>.
	/// Faster than <see cref="Random"/>.
	/// </summary>
	public sealed class UIntRng
	{
		// Add 2 after we convert to a double, otherwise we'll overflow
		private const double UIntMaxDouble = uint.MaxValue;
		private uint seed;
		/// <summary>
		/// Creates a new instance. Uses <see cref="Guid.ToByteArray()"/> on a new <see cref="Guid"/> to get the initial seed value.
		/// </summary>
		public UIntRng()
		{
			seed = BinaryPrimitives.ReadUInt32LittleEndian(Guid.NewGuid().ToByteArray());
		}
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="seed">The initial seed value to use</param>
		public UIntRng(uint seed)
		{
			this.seed = seed;
		}
		/// <summary>
		/// Returns a random integer.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public uint Next()
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
		public uint Next(uint minValue, uint maxValue)
		{
			double d = NextDouble();
			return (uint)(((ulong)maxValue - minValue) * d) + minValue;
		}
		/// <summary>
		/// Returns a double that is 0.0 or larger and less than 1.0.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble()
		{
			return (Next() / UIntMaxDouble);
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
				BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(i, 4), Next());
			}
			// If there's any left over, then fill the remainder
			if (i != span.Length)
			{
				Span<byte> extra = stackalloc byte[4];
				BinaryPrimitives.WriteUInt32LittleEndian(extra, Next());
				int j = 0;
				for (; i < span.Length; i++)
				{
					span[i] = extra[j++];
				}
			}
		}
	}
}
