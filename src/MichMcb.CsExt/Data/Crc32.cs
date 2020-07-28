using System;
using System.IO;
using System.Linq;

namespace MichMcb.CsExt.Data
{
	/// <summary>
	/// Calculates the CRC32 hash of a block of data
	/// </summary>
	public static class Crc32
	{
		/// <summary>
		/// Lookup table to optimize the algorithm.
		/// 0xEDB88320 is the generator polynomial (modulo 2) for the reversed CRC32 algorithm.
		/// </summary>
		private static readonly uint[] checksumTable = Enumerable.Range(0, 256).Select(i =>
			{
				uint tableEntry = (uint)i;
				for (int j = 0; j < 8; ++j)
				{
					tableEntry = ((tableEntry & 1) != 0)
						? (0xEDB88320 ^ (tableEntry >> 1))
						: (tableEntry >> 1);
				}
				return tableEntry;
			}).ToArray();
		public static uint GetCrc32(in ReadOnlySpan<byte> data)
		{
			uint crc32 = 0xFFFFFFFF;
			for (int i = 0; i < data.Length; i++)
			{
				crc32 = checksumTable[(byte)(crc32 ^ data[i])] ^ (crc32 >> 8);
			}
			return ~crc32;
		}
		public static uint GetCrc32(Stream data)
		{
			uint crc32 = 0xFFFFFFFF;
			while (true)
			{
				int b = data.ReadByte();
				if (b != -1)
				{
					crc32 = checksumTable[(byte)(crc32 ^ (uint)b)] ^ (crc32 >> 8);
				}
				else
				{
					break;
				}
			}
			return ~crc32;
		}
	}
}
