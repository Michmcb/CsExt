namespace MichMcb.CsExt
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Holds a value of <typeparamref name="T"/>, and a string representation of that value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public readonly struct NameValue<T> : IEquatable<NameValue<T>>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="name">The string representation.</param>
		public NameValue(T value, string name)
		{
			Value = value;
			Name = name;
		}
		/// <summary>
		/// The value.
		/// </summary>
		public T Value { get; }
		/// <summary>
		/// The string representation.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// Returns true if <paramref name="obj"/> is a <see cref="NameValue{T}"/>, and both <see cref="Value"/> and <see cref="Name"/> are equal.
		/// </summary>
		/// <param name="obj">The object to compare to.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public override bool Equals(object? obj)
		{
			return obj is NameValue<T> value && Equals(value);
		}
		/// <summary>
		/// Returns true if <see cref="Value"/> and <see cref="Name"/> are equal.
		/// </summary>
		/// <param name="other">The object to compare to.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public bool Equals(NameValue<T> other)
		{
			return EqualityComparer<T>.Default.Equals(Value, other.Value) &&
					 Name == other.Name;
		}
		/// <summary>
		/// Calculates hashcode based on both <see cref="Value"/> and <see cref="Name"/>.
		/// </summary>
		/// <returns>A hash code that represents this.</returns>
		public override int GetHashCode()
		{
#if NETSTANDARD2_0
			int hashCode = -1670801664;
			hashCode = hashCode * -1521134295 + EqualityComparer<T>.Default.GetHashCode(Value);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			return hashCode;
#else
			return HashCode.Combine(Value, Name);
#endif
		}
		/// <summary>
		/// Returns true if <see cref="Value"/> and <see cref="Name"/> are equal.
		/// </summary>
		public static bool operator ==(NameValue<T> left, NameValue<T> right) => left.Equals(right);
		/// <summary>
		/// Returns true if <see cref="Value"/> and <see cref="Name"/> are equal.
		/// </summary>
		public static bool operator !=(NameValue<T> left, NameValue<T> right) => !(left == right);
	}
}