namespace MichMcb.CsExt
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Wraps a single value, presenting it as an <see cref="IReadOnlyList{T}"/>.
	/// Is a class, since this will likely have to be boxed frequently.
	/// </summary>
	/// <typeparam name="T">The type to wrap.</typeparam>
	public sealed class One<T> : IReadOnlyList<T>
	{
		/// <summary>
		/// Creates a new instance with the provided value.
		/// </summary>
		/// <param name="value">The value.</param>
		public One(T value)
		{
			Value = value;
		}
		/// <summary>
		/// The value.
		/// </summary>
		public T Value { get; }
		/// <summary>
		/// Returns <see cref="Value"/> if <paramref name="index"/> is 0, throws <see cref="IndexOutOfRangeException"/> otherwise.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns><see cref="Value"/> if <paramref name="index"/> is 0.</returns>
		/// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is not 0.</exception>
		public T this[int index] => index == 0 ? Value : throw new IndexOutOfRangeException("Index must be 0 because this is a single item");
		/// <summary>
		/// Returns 1.
		/// </summary>
		public int Count => 1;
		/// <summary>
		/// Gets an enumerator that iterates once, returning <see cref="Value"/>.
		/// </summary>
		public IEnumerator<T> GetEnumerator()
		{
			return new SingleEnumerator(Value);
		}
		/// <summary>
		/// Gets an enumerator that iterates once, returning <see cref="Value"/>.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SingleEnumerator(Value);
		}
		/// <summary>
		/// Enumerates over a single <typeparamref name="T"/>, as if it were an array with 1 <typeparamref name="T"/>.
		/// </summary>
		public struct SingleEnumerator : IEnumerator<T>
		{
			private readonly T s;
			private int state;
			/// <summary>
			/// Creates a new instance with the single element <paramref name="s"/>.
			/// </summary>
			/// <param name="s">The element.</param>
			public SingleEnumerator(T s)
			{
				// We have 3 states. Before s, at s, and after s.
				// In other words, 0, 1, and >=2
				state = 0;
				this.s = s;
			}
			/// <summary>
			/// Returns the current element, or the default value for <typeparamref name="T"/> if <see cref="MoveNext"/> returned false.
			/// </summary>
			public T Current => state == 1 ? s : default!;
			/// <summary>
			/// Returns the current element, or the default value for <typeparamref name="T"/> if <see cref="MoveNext"/> returned false.
			/// </summary>
			object IEnumerator.Current => Current!;
			/// <summary>
			/// Does nothing.
			/// </summary>
			public void Dispose() { }
			/// <summary>
			/// Moves to the next element. Only returns true once.
			/// </summary>
			/// <returns>True on successfully advancing to the next element, false otherwise.</returns>
			public bool MoveNext()
			{
				// We return true once; when we move past the first element
				return state++ == 0;
			}
			/// <summary>
			/// Resets enumeration to the initial position (one before the only element).
			/// </summary>
			public void Reset()
			{
				state = 0;
			}
		}
	}
}
