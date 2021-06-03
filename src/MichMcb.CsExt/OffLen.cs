namespace MichMcb.CsExt
{
	using System;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// An alternative to <see cref="Range"/>. This represents an offset, and a length.
	/// In .netstandard2.0 and earlier, Range does not exist, so this is used in place of that.
	/// </summary>
	public readonly struct OffLen : IEquatable<OffLen>
	{
		public OffLen(int offset, int length)
		{
			Offset = offset;
			Length = length;
		}
#if !NETSTANDARD2_0
		public OffLen(Range range, int length)
		{
			(int rOffset, int rLength) = range.GetOffsetAndLength(length);
			Offset = rOffset;
			Length = rLength;
		}
#endif
		public int Offset { get; }
		public int Length { get; }
		/// <summary>
		/// Calculated as Offset + Length
		/// </summary>
		public int End => Offset + Length;
		/// <summary>
		/// Identical to creating a new instance
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OffLen OffsetLength(int offset, int length)
		{
			return new(offset, length);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static OffLen StartEnd(int start, int end)
		{
			return new(start, end - start);
		}
		public ReadOnlySpan<T> Slice<T>(in ReadOnlySpan<T> span)
		{
			return span.Slice(Offset, Length);
		}
		public void Deconstruct(out int offset, out int length)
		{
			offset = Offset;
			length = Length;
		}
		public override bool Equals(object? obj)
		{
			return obj is OffLen len && Equals(len);
		}
		public bool Equals(OffLen other)
		{
			return Offset == other.Offset && Length == other.Length;
		}
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
		public override string? ToString()
		{
			// TODO use a span for formatting OffLen, rather than string.concat
			return string.Concat(Offset, "..", Length);
		}
		public static bool operator ==(OffLen left, OffLen right) => left.Equals(right);
		public static bool operator !=(OffLen left, OffLen right) => !left.Equals(right);
#if !NETSTANDARD2_0
		public static implicit operator Range(OffLen offLen) => new(offLen.Offset, offLen.End);
#endif
	}
}
