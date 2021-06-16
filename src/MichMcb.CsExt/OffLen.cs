namespace MichMcb.CsExt
{
	using System;
	using System.Runtime.CompilerServices;

#if !NETSTANDARD2_0
	/// <summary>
	/// An alternative to <see cref="Range"/>. This represents an offset, and a length.
	/// In .netstandard2.0 and earlier, Range does not exist, so this is used in place of that.
	/// You shouldn't need to use this if you are not supporting .netstandard2.0 or earlier.
	/// </summary>
#else
	/// <summary>
	/// Represents an offset and a length.
	/// The Range type does not exist in .netstandard2.0 and earlier, so this can be used instead.
	/// </summary>
#endif
	public readonly struct OffLen : IEquatable<OffLen>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length.</param>
		public OffLen(int offset, int length)
		{
			Offset = offset;
			Length = length;
		}
#if !NETSTANDARD2_0
		/// <summary>
		/// Creates a new instance from an instance of <see cref="Range"/>.
		/// Calls <see cref="Range.GetOffsetAndLength(int)"/>, passing <paramref name="length"/>.
		/// </summary>
		/// <param name="range"></param>
		/// <param name="length"></param>
		public OffLen(Range range, int length)
		{
			(int rOffset, int rLength) = range.GetOffsetAndLength(length);
			Offset = rOffset;
			Length = rLength;
		}
#endif
		/// <summary>
		/// The offset, or the inclusive start index.
		/// </summary>
		public int Offset { get; }
		/// <summary>
		/// The length
		/// </summary>
		public int Length { get; }
		/// <summary>
		/// Calculated as Offset + Length. It's the exclusive end index.
		/// </summary>
		public int End => Offset + Length;
		/// <summary>
		/// Creates a new instance, with offset being <paramref name="start"/>, and length being <paramref name="end"/> - <paramref name="start"/>.
		/// </summary>
		/// <param name="start">The offset.</param>
		/// <param name="end">The exclusive end index.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OffLen StartEnd(int start, int end)
		{
			return new(start, end - start);
		}
		/// <summary>
		/// Takes a slice of <paramref name="span"/>, passing <see cref="Offset"/> and <see cref="Length"/>.
		/// </summary>
		/// <param name="span">The span to take a slice of.</param>
		/// <returns>A slice of <paramref name="span"/></returns>
		public ReadOnlySpan<T> Slice<T>(in ReadOnlySpan<T> span)
		{
			return span.Slice(Offset, Length);
		}
		/// <summary>
		/// Returns <see cref="Offset"/> and <see cref="Length"/>.
		/// </summary>
		public void Deconstruct(out int offset, out int length)
		{
			offset = Offset;
			length = Length;
		}
		/// <summary>
		/// Returns true if <paramref name="obj"/> is <see cref="OffLen"/>, and both <see cref="Offset"/> and <see cref="Length"/> are equal.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public override bool Equals(object? obj)
		{
			return obj is OffLen len && Equals(len);
		}
		/// <summary>
		/// Returns true if both <see cref="Offset"/> and <see cref="Length"/> are equal.
		/// </summary>
		/// <param name="other">The <see cref="OffLen"/> to compare against.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public bool Equals(OffLen other)
		{
			return Offset == other.Offset && Length == other.Length;
		}
		/// <summary>
		/// Returns a hashcode based on <see cref="Offset"/> and <see cref="Length"/>.
		/// </summary>
		/// <returns>A hashcode.</returns>
#if !NETSTANDARD2_0
		public override int GetHashCode()
		{
			return HashCode.Combine(Offset, Length);
		}
#else
		public override int GetHashCode()
		{
			int hashCode = -1016213585;
			hashCode = hashCode * -1521134295 + Offset.GetHashCode();
			hashCode = hashCode * -1521134295 + Length.GetHashCode();
			return hashCode;
		}
#endif
		/// <summary>
		/// Returns a string like 0..10 (where 0 is <see cref="Offset"/> and 10 is <see cref="End"/>)
		/// </summary>
		/// <returns>A string representation.</returns>
		public override string? ToString()
		{
			return string.Concat(Offset, "..", End);
		}
		/// <summary>
		/// Returns true if both <see cref="Offset"/> and <see cref="Length"/> are equal.
		/// </summary>
		public static bool operator ==(OffLen left, OffLen right) => left.Equals(right);
		/// <summary>
		/// Returns true if either <see cref="Offset"/> and <see cref="Length"/> or inequal.
		/// </summary>
		public static bool operator !=(OffLen left, OffLen right) => !left.Equals(right);
#if !NETSTANDARD2_0
		/// <summary>
		/// Creates a new <see cref="Range"/> from <see cref="Offset"/> and <see cref="End"/>.
		/// </summary>
		/// <param name="offLen">The object to cast to <see cref="Range"/>.</param>
		public static implicit operator Range(OffLen offLen) => new(offLen.Offset, offLen.End);
#endif
	}
}
