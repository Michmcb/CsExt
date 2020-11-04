using System;

namespace MichMcb.CsExt.Data
{
	/// <summary>
	/// Converts primitive types into little or big endian.
	/// No bounds checking is done for you; make sure that the passed buffers are large enough!
	/// </summary>
	public static class BitConvert
	{
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(short val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
		}
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(ushort val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
		}
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(int val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
		}
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(uint val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
		}
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(long val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
			buf[4] = (byte)(val >> 32);
			buf[5] = (byte)(val >> 40);
			buf[6] = (byte)(val >> 48);
			buf[7] = (byte)(val >> 56);
		}
		/// <summary>
		/// Writes the little-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the little-endian representation of <paramref name="val"/>.</param>
		public static void LittleEndian(ulong val, in Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
			buf[4] = (byte)(val >> 32);
			buf[5] = (byte)(val >> 40);
			buf[6] = (byte)(val >> 48);
			buf[7] = (byte)(val >> 56);
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a short, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read a short from.</param>
		/// <returns>The read value.</returns>
		public static short LittleEndianShort(in ReadOnlySpan<byte> buf)
		{
			int val = 0;
			val |= buf[0];
			val |= buf[1] << 8;
			return (short)val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a ushort, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read a ushort from.</param>
		/// <returns>The read value.</returns>
		public static ushort LittleEndianUShort(in ReadOnlySpan<byte> buf)
		{
			uint val = 0;
			val |= buf[0];
			val |= (uint)buf[1] << 8;
			return (ushort)val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as an int, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read an int from.</param>
		/// <returns>The read value.</returns>
		public static int LittleEndianInt(in ReadOnlySpan<byte> buf)
		{
			int val = 0;
			val |= buf[0];
			val |= buf[1] << 8;
			val |= buf[2] << 16;
			val |= buf[3] << 24;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a uint, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read a uint from.</param>
		/// <returns>The read value.</returns>
		public static uint LittleEndianUInt(in ReadOnlySpan<byte> buf)
		{
			uint val = 0;
			val |= buf[0];
			val |= (uint)buf[1] << 8;
			val |= (uint)buf[2] << 16;
			val |= (uint)buf[3] << 24;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a long, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read a long from.</param>
		/// <returns>The read value.</returns>
		public static long LittleEndianLong(in ReadOnlySpan<byte> buf)
		{
			long val = 0;
			val |= buf[0];
			val |= (long)buf[1] << 8;
			val |= (long)buf[2] << 16;
			val |= (long)buf[3] << 24;
			val |= (long)buf[4] << 32;
			val |= (long)buf[5] << 40;
			val |= (long)buf[6] << 48;
			val |= (long)buf[7] << 56;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a ulong, stored in little-endian format.
		/// </summary>
		/// <param name="buf">The span to read a ulong from.</param>
		/// <returns>The read value.</returns>
		public static ulong LittleEndianULong(in ReadOnlySpan<byte> buf)
		{
			ulong val = 0;
			val |= buf[0];
			val |= (ulong)buf[1] << 8;
			val |= (ulong)buf[2] << 16;
			val |= (ulong)buf[3] << 24;
			val |= (ulong)buf[4] << 32;
			val |= (ulong)buf[5] << 40;
			val |= (ulong)buf[6] << 48;
			val |= (ulong)buf[7] << 56;
			return val;
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(short val, in Span<byte> buf)
		{
			buf[1] = (byte)(val);
			buf[0] = (byte)(val >> 8);
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(ushort val, in Span<byte> buf)
		{
			buf[1] = (byte)(val);
			buf[0] = (byte)(val >> 8);
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(int val, in Span<byte> buf)
		{
			buf[3] = (byte)(val);
			buf[2] = (byte)(val >> 8);
			buf[1] = (byte)(val >> 16);
			buf[0] = (byte)(val >> 24);
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(uint val, in Span<byte> buf)
		{
			buf[3] = (byte)(val);
			buf[2] = (byte)(val >> 8);
			buf[1] = (byte)(val >> 16);
			buf[0] = (byte)(val >> 24);
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(long val, in Span<byte> buf)
		{
			buf[7] = (byte)val;
			buf[6] = (byte)(val >> 8);
			buf[5] = (byte)(val >> 16);
			buf[4] = (byte)(val >> 24);
			buf[3] = (byte)(val >> 32);
			buf[2] = (byte)(val >> 40);
			buf[1] = (byte)(val >> 48);
			buf[0] = (byte)(val >> 56);
		}
		/// <summary>
		/// Writes the big-endian representation of <paramref name="val"/> to <paramref name="buf"/>.
		/// </summary>
		/// <param name="val">The value.</param>
		/// <param name="buf">The span to use to write the big-endian representation of <paramref name="val"/>.</param>
		public static void BigEndian(ulong val, in Span<byte> buf)
		{
			buf[7] = (byte)val;
			buf[6] = (byte)(val >> 8);
			buf[5] = (byte)(val >> 16);
			buf[4] = (byte)(val >> 24);
			buf[3] = (byte)(val >> 32);
			buf[2] = (byte)(val >> 40);
			buf[1] = (byte)(val >> 48);
			buf[0] = (byte)(val >> 56);
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a short, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read a short from.</param>
		/// <returns>The read value.</returns>
		public static short BigEndianShort(in ReadOnlySpan<byte> buf)
		{
			int val = 0;
			val |= buf[1];
			val |= buf[0] << 8;
			return (short)val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a ushort, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read a ushort from.</param>
		/// <returns>The read value.</returns>
		public static ushort BigEndianUShort(in ReadOnlySpan<byte> buf)
		{
			uint val = 0;
			val |= buf[1];
			val |= (uint)buf[0] << 8;
			return (ushort)val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as an int, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read an int from.</param>
		/// <returns>The read value.</returns>
		public static int BigEndianInt(in ReadOnlySpan<byte> buf)
		{
			int val = 0;
			val |= buf[3];
			val |= buf[2] << 8;
			val |= buf[1] << 16;
			val |= buf[0] << 24;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as an uint, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read an uint from.</param>
		/// <returns>The read value.</returns>
		public static uint BigEndianUInt(in ReadOnlySpan<byte> buf)
		{
			uint val = 0;
			val |= buf[3];
			val |= (uint)buf[2] << 8;
			val |= (uint)buf[1] << 16;
			val |= (uint)buf[0] << 24;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a long, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read a long from.</param>
		/// <returns>The read value.</returns>
		public static long BigEndianLong(in ReadOnlySpan<byte> buf)
		{
			long val = 0;
			val |= buf[7];
			val |= (long)buf[6] << 8;
			val |= (long)buf[5] << 16;
			val |= (long)buf[4] << 24;
			val |= (long)buf[3] << 32;
			val |= (long)buf[2] << 40;
			val |= (long)buf[1] << 48;
			val |= (long)buf[0] << 56;
			return val;
		}
		/// <summary>
		/// Reads <paramref name="buf"/> as a ulong, stored in big-endian format.
		/// </summary>
		/// <param name="buf">The span to read a ulong from.</param>
		/// <returns>The read value.</returns>
		public static ulong BigEndianULong(in ReadOnlySpan<byte> buf)
		{
			ulong val = 0;
			val |= buf[7];
			val |= (ulong)buf[6] << 8;
			val |= (ulong)buf[5] << 16;
			val |= (ulong)buf[4] << 24;
			val |= (ulong)buf[3] << 32;
			val |= (ulong)buf[2] << 40;
			val |= (ulong)buf[1] << 48;
			val |= (ulong)buf[0] << 56;
			return val;
		}
	}
}