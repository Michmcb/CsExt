namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Buffers.Binary;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// A random number generator, implemented as a permuted congruential generator.
	/// Ported from here: https://www.pcg-random.org/download.html
	/// </summary>
	public sealed class PcgRng
	{
		private const double UIntMaxDouble = uint.MaxValue + 1d;
		private ulong state;
		private readonly ulong increment;
		/// <summary>
		/// Creates a new instance. Uses <see cref="Guid.ToByteArray()"/> on a new <see cref="Guid"/> to get the initial seed/increment value.
		/// </summary>
		public PcgRng()
		{
			Span<byte> init = Guid.NewGuid().ToByteArray();
			state = 0;
			// inc always has to be an odd number
			increment = (BinaryPrimitives.ReadUInt64LittleEndian(init.Slice(8)) << 1) | 1;
			NextUInt32();
			state += BinaryPrimitives.ReadUInt64LittleEndian(init);
			NextUInt32();
		}
		/// <summary>
		/// Creates a new instance with the provided state and increment. Note, <paramref name="increment"/> must be odd (it's forced to be odd).
		/// </summary>
		/// <param name="state">The initial state.</param>
		/// <param name="increment">The increment, which must be odd. It will be forced to be odd if it is not.</param>
		public PcgRng(ulong state, ulong increment)
		{
			this.state = state;
			this.increment = increment | 1;
		}
		/// <summary>
		/// Returns a random integer.
		/// </summary>
		public int NextInt32()
		{
			// Subtract 1 more than int.MaxValue which will move the value into the correct range
			return (int)((long)NextUInt32() - 2147483648);
		}
		/// <summary>
		/// Returns a random integer.
		/// </summary>
		public uint NextUInt32()
		{
			unchecked
			{
				ulong old = state;
				state = old * 6364136223846793005L + increment;
				uint xorshifted = (uint)(((old >> 18) ^ old) >> 27);
				int rot = (int)(old >> 59);
				return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
			}
		}
		/// <summary>
		/// Returns a random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
		/// If <paramref name="maxValue"/> equals <paramref name="minValue"/>, returns <paramref name="minValue"/>.
		/// If <paramref name="minValue"/> is larger than <paramref name="maxValue"/>, throws <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="minValue">The inclusive minimum value.</param>
		/// <param name="maxValue">The exclusive maximum value.</param>
		/// <returns>A random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is larger than <paramref name="maxValue"/></exception>
		public int NextInt32(int minValue, int maxValue)
		{
			return minValue <= maxValue
				? minValue != maxValue
					? (int)(((long)maxValue - minValue) * NextDouble()) + minValue
					: minValue
				: throw new ArgumentOutOfRangeException(nameof(minValue), "minValue is larger than maxValue");
		}
		/// <summary>
		/// Returns a random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
		/// If <paramref name="maxValue"/> equals <paramref name="minValue"/>, returns <paramref name="minValue"/>.
		/// If <paramref name="minValue"/> is larger than <paramref name="maxValue"/>, throws <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="minValue">The inclusive minimum value.</param>
		/// <param name="maxValue">The exclusive maximum value.</param>
		/// <returns>A random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is larger than <paramref name="maxValue"/></exception>
		public uint NextUInt32(uint minValue, uint maxValue)
		{
			return minValue <= maxValue
				? minValue != maxValue
					? (uint)(((ulong)maxValue - minValue) * NextDouble()) + minValue
					: minValue
				: throw new ArgumentOutOfRangeException(nameof(minValue), "minValue is larger than maxValue");
		}
		/// <summary>
		/// Returns a double that is 0.0 or larger and less than 1.0.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double NextDouble()
		{
			// Using this for a clamped range is faster than doing the method for debiased integer multiplication (by Lemire)
			return NextUInt32() / UIntMaxDouble;
		}
		/// <summary>
		/// Fills <paramref name="span"/> with random bytes.
		/// </summary>
		public void NextBytes(in Span<byte> span)
		{
			int i;
			// We'll fill the span in 4-byte increments 
			for (i = 0; i < span.Length - 3; i += 4)
			{
				BinaryPrimitives.WriteUInt32LittleEndian(span.Slice(i, 4), NextUInt32());
			}
			// If there's any left over, then fill the remainder
			if (i != span.Length)
			{
				Span<byte> extra = stackalloc byte[4];
				BinaryPrimitives.WriteUInt32LittleEndian(extra, NextUInt32());
				int j = 0;
				for (; i < span.Length; i++)
				{
					span[i] = extra[j++];
				}
			}
		}
	}
}
