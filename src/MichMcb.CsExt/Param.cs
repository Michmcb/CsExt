namespace MichMcb.CsExt
{
	using System;
	/// <summary>
	/// Simple parameter checking; throws exceptions when a parameter does not satisfy the stated condition.
	/// </summary>
	public static class Param
	{
		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if <paramref name="arg"/> is null.
		/// </summary>
		public static void NotNull<T>(T arg, string paramName, string msg) where T : class
		{
			if (arg == null)
			{
				throw new ArgumentNullException(paramName, msg);
			}
		}
		/// <summary>
		/// Throws an <see cref="ArgumentException"/> if <paramref name="isValid"/> is false.
		/// </summary>
		public static void Valid(bool isValid, string paramName, string msg)
		{
			if (!isValid)
			{
				throw new ArgumentException(msg, paramName);
			}
		}
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is larger than <paramref name="largerThan"/> or less than <paramref name="lessThan"/>.
		/// </summary>
		public static void InClosedRange<T>(T value, T largerThan, T lessThan, string paramName, string msg) where T : IComparable<T>
		{
			// value compared to larger than must 1 or more
			// And, value compared to less than must be -1 or less
			if (value.CompareTo(largerThan) <= 0 || value.CompareTo(lessThan) >= 0)
			{
				throw new ArgumentOutOfRangeException(paramName, msg);
			}
		}
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is larger than <paramref name="largerThan"/> or less than or equal to <paramref name="lessThanOrEqualTo"/>.
		/// </summary>
		public static void InHalfOpenRange<T>(T value, T largerThan, T lessThanOrEqualTo, string paramName, string msg) where T : IComparable<T>
		{
			// value compared to larger than must 1 or more
			// And, value compared to less than must be 0 or less
			if (value.CompareTo(largerThan) <= 0 || value.CompareTo(lessThanOrEqualTo) >= 1)
			{
				throw new ArgumentOutOfRangeException(paramName, msg);
			}
		}
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="value"/> is larger than or equal to <paramref name="largerThanOrEqualTo"/> or less than or equal to <paramref name="lessThanOrEqualTo"/>.
		/// </summary>
		public static void InOpenRange<T>(T value, T largerThanOrEqualTo, T lessThanOrEqualTo, string paramName, string msg) where T : IComparable<T>
		{
			// value compared to larger than must 1 or more
			// And, value compared to less than must be 0 or less
			if (value.CompareTo(largerThanOrEqualTo) <= -1 || value.CompareTo(lessThanOrEqualTo) >= 1)
			{
				throw new ArgumentOutOfRangeException(paramName, msg);
			}
		}
		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException"/> if <paramref name="isInRange"/> is false.
		/// </summary>
		public static void InRange(bool isInRange, string paramName, string msg)
		{
			if (!isInRange)
			{
				throw new ArgumentOutOfRangeException(paramName, msg);
			}
		}
	}
}
