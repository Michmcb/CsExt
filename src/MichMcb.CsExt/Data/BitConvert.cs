using System;

namespace MichMcb.CsExt.Data
{
	/// <summary>
	/// Converts primitive types into little or big endian.
	/// </summary>
	public static class BitConvert
	{
		public static void LittleEndian(short val, Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
		}
		public static void LittleEndian(ushort val, Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
		}
		public static void LittleEndian(int val, Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
		}
		public static void LittleEndian(uint val, Span<byte> buf)
		{
			buf[0] = (byte)val;
			buf[1] = (byte)(val >> 8);
			buf[2] = (byte)(val >> 16);
			buf[3] = (byte)(val >> 24);
		}
		public static void LittleEndian(long val, Span<byte> buf)
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
		public static void LittleEndian(ulong val, Span<byte> buf)
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
		public static short LittleEndianShort(Span<byte> buf)
		{
			int val = 0;
			val |= buf[0];
			val |= buf[1] << 8;
			return (short)val;
		}
		public static int LittleEndianInt(Span<byte> buf)
		{
			int val = 0;
			val |= buf[0];
			val |= buf[1] << 8;
			val |= buf[2] << 16;
			val |= buf[3] << 24;
			return val;
		}
		public static long LittleEndianLong(Span<byte> buf)
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
		public static void BigEndian(short val, Span<byte> buf)
		{
			buf[1] = (byte)(val);
			buf[0] = (byte)(val >> 8);
		}
		public static void BigEndian(ushort val, Span<byte> buf)
		{
			buf[1] = (byte)(val);
			buf[0] = (byte)(val >> 8);
		}
		public static void BigEndian(int val, Span<byte> buf)
		{
			buf[3] = (byte)(val);
			buf[2] = (byte)(val >> 8);
			buf[1] = (byte)(val >> 16);
			buf[0] = (byte)(val >> 24);
		}
		public static void BigEndian(uint val, Span<byte> buf)
		{
			buf[3] = (byte)(val);
			buf[2] = (byte)(val >> 8);
			buf[1] = (byte)(val >> 16);
			buf[0] = (byte)(val >> 24);
		}
		public static void BigEndian(long val, Span<byte> buf)
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
		public static void BigEndian(ulong val, Span<byte> buf)
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
		public static short BigEndianShort(Span<byte> buf)
		{
			int val = 0;
			val |= buf[1];
			val |= buf[0] << 8;
			return (short)val;
		}
		public static int BigEndianInt(Span<byte> buf)
		{
			int val = 0;
			val |= buf[3];
			val |= buf[2] << 8;
			val |= buf[1] << 16;
			val |= buf[0] << 24;
			return val;
		}
		public static long BigEndianLong(Span<byte> buf)
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
	}
}