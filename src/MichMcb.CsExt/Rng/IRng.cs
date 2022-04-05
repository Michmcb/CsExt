namespace MichMcb.CsExt.Rng
{
	using System;

	/// <summary>
	/// An interface for a random number generator
	/// </summary>
	public interface IRng
	{
		/// <summary>
		/// Returns a random integer.
		/// </summary>
		int NextInt32();
		/// <summary>
		/// Returns a random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
		/// If <paramref name="maxValue"/> equals <paramref name="minValue"/>, returns <paramref name="minValue"/>.
		/// If <paramref name="minValue"/> is larger than <paramref name="maxValue"/>, throws <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="minValue">The inclusive minimum value.</param>
		/// <param name="maxValue">The exclusive maximum value.</param>
		/// <returns>A random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is larger than <paramref name="maxValue"/></exception>
		int NextInt32(int minValue, int maxValue);

		/// <summary>
		/// Returns a random integer.
		/// </summary>
		uint NextUInt32();
		/// <summary>
		/// Returns a random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.
		/// If <paramref name="maxValue"/> equals <paramref name="minValue"/>, returns <paramref name="minValue"/>.
		/// If <paramref name="minValue"/> is larger than <paramref name="maxValue"/>, throws <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <param name="minValue">The inclusive minimum value.</param>
		/// <param name="maxValue">The exclusive maximum value.</param>
		/// <returns>A random integer that is equal to or greater than <paramref name="minValue"/>, and less than <paramref name="maxValue"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is larger than <paramref name="maxValue"/></exception>
		uint NextUInt32(uint minValue, uint maxValue);
		/// <summary>
		/// Returns a double that is 0.0 or larger and less than 1.0.
		/// </summary>
		double NextDouble();
		/// <summary>
		/// Gets a number of random bytes.
		/// </summary>
		byte[] NextBytes(int count);
		/// <summary>
		/// Fills <paramref name="span"/> with random bytes.
		/// </summary>
		void FillBytes(Span<byte> span);
	}
}
