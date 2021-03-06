﻿namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// A random number generator that produces <see cref="uint"/>.
	/// Much faster than <see cref="Random"/>.
	/// </summary>
	public sealed class UIntRng
	{
		// Add 2 after we convert to a double, otherwise we'll overflow
		private const double DoubleIntMaxValue = (uint.MaxValue * 2d) + 2d;
		private uint seed;
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
			return (Next() / DoubleIntMaxValue) + 0.5d;
		}
	}
}
