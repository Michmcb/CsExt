#if !NETSTANDARD2_0
namespace MichMcb.CsExt.Data
{
	using System;
	using System.Buffers.Binary;
	/// <summary>
	/// A struct representing an MD5 hash
	/// </summary>
	public readonly struct Md5 : IEquatable<Md5>
	{
		/// <summary>
		/// Creates a new MD5 from <paramref name="raw"/>, which must be exactly 16 bytes long.
		/// </summary>
		/// <param name="raw"></param>
		public Md5(in ReadOnlySpan<byte> raw)
		{
			if (raw.Length != 16)
			{
				throw new ArgumentException("Provided bytes were not exactly 16 long", nameof(raw));
			}

			Upper8 = BinaryPrimitives.ReadInt64LittleEndian(raw);
			Lower8 = BinaryPrimitives.ReadInt64LittleEndian(raw[8..]);
		}
		/// <summary>
		/// The upper 8 bytes.
		/// </summary>
		public long Upper8 { get; }
		/// <summary>
		/// The lower 8 bytes.
		/// </summary>
		public long Lower8 { get; }
		/// <summary>
		/// Creates a new array which represents this MD5.
		/// </summary>
		/// <returns>An new array representing this MD5.</returns>
		public byte[] ToArray()
		{
			byte[] raw = new byte[16];
			BinaryPrimitives.WriteInt64LittleEndian(raw, Upper8);
			BinaryPrimitives.WriteInt64LittleEndian(raw.AsSpan()[8..], Lower8);
			return raw;
		}
		/// <summary>
		/// Fills <paramref name="target"/> with bytes which represents this MD5.
		/// </summary>
		/// <param name="target">The span to fill with bytes.</param>
		public void FillSpan(in Span<byte> target)
		{
			BinaryPrimitives.WriteInt64LittleEndian(target, Upper8);
			BinaryPrimitives.WriteInt64LittleEndian(target[8..], Lower8);
		}
		/// <summary>
		/// Returns true if <paramref name="obj"/> is an <see cref="Md5"/> and it is equal to this.
		/// </summary>
		/// <param name="obj">Object to compare.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public override bool Equals(object? obj)
		{
			return obj is Md5 md && Equals(md);
		}
		/// <summary>
		/// Returns true if <paramref name="other"/> is equal to this.
		/// </summary>
		/// <param name="other">Object to compare.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public bool Equals(Md5 other)
		{
			return Upper8 == other.Upper8 && Lower8 == other.Lower8;
		}
		/// <summary>
		/// Returns a hashcode for this.
		/// </summary>
		/// <returns>A hashcode representing this.</returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Upper8, Lower8);
		}
		/// <summary>
		/// Returns an uppercase hexadecimal string, without leading 0x.
		/// </summary>
		/// <returns>Hex string representing this</returns>
		public override string? ToString()
		{
			return Upper8.ToString("X8") + Lower8.ToString("X8");
		}
		/// <summary>
		/// Returns a hexadecimal string.
		/// </summary>
		/// <param name="leading0x">If true, has a leading 0x. False otherwise.</param>
		/// <param name="uppercase">Uppercase or lowercase.</param>
		/// <returns>Hex string representing this</returns>
		public string? ToString(bool leading0x, bool uppercase)
		{
			return uppercase
				? (leading0x ? "0x" : string.Empty) + Upper8.ToString("X8") + Lower8.ToString("X8")
				: (leading0x ? "0x" : string.Empty) + Upper8.ToString("x8") + Lower8.ToString("x8");
		}
		/// <summary>
		/// Does the same thing as <see cref="Equals(Md5)"/>.
		/// </summary>
		/// <param name="left">Left hand side.</param>
		/// <param name="right">Right hand side.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public static bool operator ==(Md5 left, Md5 right) => left.Equals(right);
		/// <summary>
		/// Does the same thing as !<see cref="Equals(Md5)"/>.
		/// </summary>
		/// <param name="left">Left hand side.</param>
		/// <param name="right">Right hand side.</param>
		/// <returns>false if equal, true otherwise.</returns>
		public static bool operator !=(Md5 left, Md5 right) => !left.Equals(right);
	}
}
#endif