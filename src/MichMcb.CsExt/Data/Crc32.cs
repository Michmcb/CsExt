namespace MichMcb.CsExt.Data
{
	using System;
	using System.IO;

	/// <summary>
	/// Calculates the CRC32 hash of a block of data.
	/// </summary>
	[Obsolete("Use Crc32.NET instead, it's much faster/better")]
	public static class Crc32
	{
		/// <summary>
		/// The initial value a CRC32 starts with.
		/// Also to finalize a CRC32, you need to flip its bytes: <code>~crc32</code>
		/// </summary>
		public const uint InitialValue = 0xFFFFFFFF;
		/// <summary>
		/// Lookup table to optimize the algorithm.
		/// 0xEDB88320 is the generator polynomial (modulo 2) for the reversed CRC32 algorithm.
		/// </summary>
		private static readonly uint[] checksumTable = GetChecksumTable();
		/// <summary>
		/// Generates a checksum table. It's a lookup table to optimize the CRC32 algorithm.
		/// Its length is always 256.
		/// </summary>
		public static uint[] GetChecksumTable()
		{
			uint[] table = new uint[256];
			for (uint i = 0; i < 256; i++)
			{
				uint tableEntry = i;
				for (int j = 0; j < 8; ++j)
				{
					tableEntry = ((tableEntry & 1) != 0)
						? (0xEDB88320u ^ (tableEntry >> 1))
						: (tableEntry >> 1);
				}
				table[i] = tableEntry;
			}
			return table;
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="data"/>, starting with <paramref name="crc32"/> as the initial value.
		/// </summary>
		/// <param name="data">The data to calculate a CRC32 for.</param>
		/// <param name="crc32">The current CRC32 calculated so far.</param>
		/// <returns>The new CRC32, not finalized.</returns>
		public static uint GetCrc32(in ReadOnlySpan<byte> data, uint crc32)
		{
			for (int i = 0; i < data.Length; i++)
			{
				crc32 = checksumTable[(byte)(crc32 ^ data[i])] ^ (crc32 >> 8);
			}
			return crc32;
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="data"/>.
		/// </summary>
		/// <param name="data">The data to calculate a CRC32 for.</param>
		/// <returns>The CRC32, finalized.</returns>
		public static uint GetCrc32(in ReadOnlySpan<byte> data)
		{
			return ~GetCrc32(data, InitialValue);
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="data"/>, starting with <paramref name="crc32"/> as the initial value.
		/// </summary>
		/// <param name="data">The data to calculate a CRC32 for.</param>
		/// <param name="crc32">The current CRC32 calculated so far.</param>
		/// <param name="offset">The index to start calculating the CRC32.</param>
		/// <param name="length">How many bytes of <paramref name="data"/> to use to calculate the CRC32</param>
		/// <returns>The new CRC32, not finalized.</returns>
		public static uint GetCrc32(byte[] data, uint crc32, int offset, int length)
		{
			for (int i = offset; i < offset + length; i++)
			{
				crc32 = checksumTable[(byte)(crc32 ^ data[i])] ^ (crc32 >> 8);
			}
			return crc32;
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="data"/>.
		/// </summary>
		/// <param name="data">The data to calculate a CRC32 for.</param>
		/// <returns>The CRC32, finalized.</returns>
		public static uint GetCrc32(byte[] data)
		{
			return ~GetCrc32(data, InitialValue, 0, data.Length);
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="stream"/>, starting with <paramref name="crc32"/> as the initial value.
		/// Reads data from <paramref name="stream"/> until the end is reached.
		/// </summary>
		/// <param name="stream">The data to calculate a CRC32 for.</param>
		/// <param name="crc32">The current CRC32 calculated so far.</param>
		/// <returns>The CRC32, not finalized.</returns>
		public static uint GetCrc32(Stream stream, uint crc32)
		{
			while (true)
			{
				int b = stream.ReadByte();
				if (b != -1)
				{
					crc32 = checksumTable[(byte)(crc32 ^ (uint)b)] ^ (crc32 >> 8);
				}
				else
				{
					return crc32;
				}
			}
		}
		/// <summary>
		/// Calculates the CRC32 of <paramref name="stream"/>.
		/// Reads data from <paramref name="stream"/> until the end is reached.
		/// </summary>
		/// <param name="stream">The data to calculate a CRC32 for.</param>
		/// <returns>The CRC32, finalized.</returns>
		public static uint GetCrc32(Stream stream)
		{
			return ~GetCrc32(stream, InitialValue);
		}
	}
}
