namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Buffers.Binary;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// A random number generator, implemented as a 64-bit linear congruential generator.
	/// Only the higher bits are returned when requesting 32-bit integers, so the potentially weakest bits are discarded.
	/// </summary>
	public sealed class LcgRng : IRng
	{
		/// <summary>
		/// Obtained from here https://onlinelibrary.wiley.com/doi/10.1002/spe.3030
		/// This minus 1 is divisible by 4.
		/// </summary>
		public const ulong a = 15074714826142052245;
		/// <summary>
		/// According to https://en.wikipedia.org/wiki/Linear_congruential_generator
		/// m is 2^32.
		/// a and c must be relatively prime (if the only positive integer that is a divisor of both of them is 1, but the numbers themselves don't have to be prime)
		/// Factors of 6387 are 1, 3, 2129, 6387, none of which can divide a without remainder.
		/// a-1 must be divisible by all prime factors of m (m would be 2^32, and the only prime factor of that would be 2)
		/// a-1 must be divisible by 4 (15074714826142052244 is indeed divisible by 4, but not by 8, 16, 32, etc. which is good)
		/// </summary>
		private const ulong c = 6387;
		private ulong state;
		/// <summary>
		/// Creates a new instance. Uses <see cref="Guid.ToByteArray()"/> on a new <see cref="Guid"/> to get the initial state.
		/// </summary>
		public LcgRng()
		{
			state = BinaryPrimitives.ReadUInt64LittleEndian(Guid.NewGuid().ToByteArray()) | 1;
		}
		/// <summary>
		/// Creates a new instance with the provided state and increment. Note, <paramref name="state"/> must be odd (it's forced to be odd).
		/// </summary>
		/// <param name="state">The initial state, which must be odd. It will be forced to be odd if it is not.</param>
		public LcgRng(ulong state)
		{
			// should be odd
			this.state = state | 1;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Next()
		{
			unchecked
			{
				state = state * a + c;
			}
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
			Next();
			return (uint)(state >> 32);
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
				? (int)(((long)maxValue - minValue) * NextDouble()) + minValue
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
				? (uint)(((ulong)maxValue - minValue) * NextDouble()) + minValue
				: throw new ArgumentOutOfRangeException(nameof(minValue), "minValue is larger than maxValue");
		}
		/// <summary>
		/// Returns a double that is 0.0 or larger and less than 1.0.
		/// </summary>
		public double NextDouble()
		{
			// Using this for a clamped range is faster than doing the method for debiased integer multiplication (by Lemire)
			return NextUInt32() / RngConst.UIntMaxValuePlusOne;
		}
		/// <summary>
		/// Gets a number of random bytes.
		/// </summary>
		public byte[] NextBytes(int count)
		{
			byte[] b = new byte[count];
			FillBytes(b);
			return b;
		}
		/// <summary>
		/// Fills <paramref name="span"/> with random bytes.
		/// </summary>
		public void FillBytes(Span<byte> span)
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
