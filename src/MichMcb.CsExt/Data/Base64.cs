namespace MichMcb.CsExt.Data
{
	using System;
	using static MichMcb.CsExt.Functions;

	/// <summary>
	/// Encodes and decodes base64 strings. Can use whatever charset you want.
	/// This is intended to be created once and reused throughout the whole of your application a bunch, since creating an instance allocates two arrays.
	/// </summary>
	public sealed class Base64
	{
		/// <summary>
		/// The standard Base64 charset.
		/// </summary>
		public const string CharsetStandard = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
		/// <summary>
		/// An alternative Base64 charset which is safe to use in URLs and filenames.
		/// </summary>
		public const string CharsetUrlSafe = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
		//private readonly char[] UrlSafeChars = new char[64]
		//{
		//	'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
		//	'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
		//	'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'
		//};
		//private readonly byte[] Base64Decode = new byte[256]
		//{
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255,
		//	62, // -
		//	255, 255,
		//	52, 53, 54, 55, 56, 57, 58, 59, 60, 61, // 0-9
		//	255, 255, 255,
		//	255, // = (We never actually decode these, we trim them off the end. Because = is not valid inside a string, we treat it like any other invalid character)
		//	255, 255, 255,
		//	0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, // A-Z
		//	255, 255, 255, 255,
		//	63, // _
		//	255,
		//	26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, // a-z
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255,
		//	255, 255, 255, 255, 255, 255, 255
		//};
		private readonly char[] charset;
		private readonly byte[] decode;
		/// <summary>
		/// Creates a new instance of Base64, which uses the standard encoding (i.e. + and / for the 63rd and 64th chars)
		/// </summary>
		public Base64() : this(CharsetStandard) { }
		/// <summary>
		/// Creates a new instance of Base64, configured to encode and decode using the provided charset.
		/// Use one of the public const strings in this class as the argument, such as <see cref="CharsetUrlSafe"/>.
		/// </summary>
		/// <param name="charset">The charset to use. Must be exactly 64 chars long.</param>
		public Base64(string charset)
		{
			if (charset.Length != 64)
			{
				throw new ArgumentException("charset must be exactly 64 chars long", nameof(charset));
			}
			this.charset = new char[64];
			decode = new byte[256];
			// Fill the array with the invalid values
			for (int i = 0; i < 256; i++)
			{
				decode[i] = 255;
			}
			for (int i = 0; i < 64; i++)
			{
				char c = charset[i];
				if (c < 0 || c > 255)
				{
					throw new ArgumentException("charset contains a character that's not in the range of 0-255: " + c, nameof(charset));
				}
				decode[c] = (byte)i;
				this.charset[i] = charset[i];
			}
		}
		// TODO possible overflow for excessively large base64 strings
		// TODO Encode to streams for Base64
		public static int BytesNeeded(int base64Length)
		{
			int needed = Math.DivRem(base64Length * 3, 4, out int remainder);
			return remainder == 0 ? needed : needed + 1;
		}
		public static int CharsNeeded(int dataLength)
		{
			int needed = Math.DivRem(dataLength * 4, 3, out int remainder);
			return remainder == 0 ? needed : needed + 1;
		}
		public int Encode(in ReadOnlySpan<byte> data, Span<char> encoded)
		{
			int w = 0;
			int i = 0;
			for (int pos = 0; pos < data.Length / 3; pos++)
			{
				// 6 upper bits
				byte d1 = (byte)((data[i] & 0b1111_1100) >> 2);
				// 2 lower bits, 4 upper bits
				byte d2 = (byte)(((data[i] & 0b0000_0011) << 4) | ((data[++i] & 0b1111_0000) >> 4));
				// 4 lower bits, 2 upper bits
				byte d3 = (byte)(((data[i] & 0b0000_1111) << 2) | ((data[++i] & 0b1100_0000) >> 6));
				// 6 lower bits
				byte d4 = (byte)(data[i++] & 0b0011_1111);

				encoded[w++] = charset[d1];
				encoded[w++] = charset[d2];
				encoded[w++] = charset[d3];
				encoded[w++] = charset[d4];
			}

			switch (data.Length - i)
			{
				// Nothing extra to write if it's 0
				case 1:
					{
						// 1 more byte to encode, which means 2 more base64 chars to write
						// Six upper bytes
						byte d1 = (byte)((data[i] & 0b1111_1100) >> 2);
						// Two lower bytes
						byte d2 = (byte)((data[i] & 0b0000_0011) << 4);
						encoded[w++] = charset[d1];
						encoded[w++] = charset[d2];
					}
					break;
				case 2:
					{
						// 2 more bytes to encode, which means 3 more base64 chars to write
						byte d1 = (byte)((data[i] & 0b1111_1100) >> 2);
						// Two lower bytes, four upper bytes
						byte d2 = (byte)(((data[i] & 0b0000_0011) << 4) | ((data[++i] & 0b1111_0000) >> 4));
						// Four lower bytes
						byte d3 = (byte)((data[i] & 0b0000_1111) << 2);
						encoded[w++] = charset[d1];
						encoded[w++] = charset[d2];
						encoded[w++] = charset[d3];
					}
					break;
			}

			return w;
		}
		public char[] Encode(in ReadOnlySpan<byte> data)
		{
			char[] base64 = new char[CharsNeeded(data.Length)];
			Encode(data, base64);
			return base64;
		}
		public string EncodeStr(in ReadOnlySpan<byte> data)
		{
			return new string(Encode(data));
		}
		public Opt<int> Decode(in ReadOnlySpan<char> base64, Span<byte> decoded)
		{
			int w = 0;
			int i = 0;
			// If we have padding, we'll just ignore it
			int len = base64.Length;
			if (base64[base64.Length - 1] == '=')
			{
				for (len = base64.Length - 1; len > 0; len--)
				{
					if (base64[len] != '=')
					{
						break;
					}
				}
				// If the whole string was padding (haha nice joke) then just return 0
				if (len == 0)
				{
					return 0;
				}
				++len;
			}

			// A value of 255 means that this is an invalid character, hence the checks against it
			for (int pos = 0; pos < len / 4; pos++)
			{
				// Each of these is a sextet, i.e. 6 bits
				byte d1 = decode[(byte)base64[i++]];
				byte d2 = decode[(byte)base64[i++]];
				byte d3 = decode[(byte)base64[i++]];
				byte d4 = decode[(byte)base64[i++]];
				if (d1 == 255 || d2 == 255 || d3 == 255 || d4 == 255)
				{
					return None(w);
				}

				// 6 of the 1st, 2 of the 2nd
				decoded[w++] = (byte)((d1 << 2) | (d2 >> 4));
				// 4 of the 2nd, 4 of the 3rd
				decoded[w++] = (byte)((d2 << 4) | (d3 >> 2));
				// 2 of the 3rd, 6 of the 4th
				decoded[w++] = (byte)((d3 << 6) | d4);
			}

			switch (len - i)
			{
				// Nothing extra to write if it's 0
				case 1:
					{
						// 1 more base64 char to decode, so that's 1 more byte
						byte d1 = decode[(byte)base64[i++]];
						if (d1 == 255)
						{
							return None(w);
						}
						decoded[w++] = (byte)(d1 << 2);
					}
					break;
				case 2:
					{
						// 2 more base64 chars to decode, so that's 1 more byte
						byte d1 = decode[(byte)base64[i++]];
						byte d2 = decode[(byte)base64[i++]];
						if (d1 == 255 || d2 == 255)
						{
							return None(w);
						}
						decoded[w++] = (byte)((d1 << 2) | (d2 >> 4));
					}
					break;
				case 3:
					{
						// 3 more base64 chars to decode, so that's 2 more bytes
						byte d1 = decode[(byte)base64[i++]];
						byte d2 = decode[(byte)base64[i++]];
						byte d3 = decode[(byte)base64[i++]];
						if (d1 == 255 || d2 == 255 || d3 == 255)
						{
							return None(w);
						}
						decoded[w++] = (byte)((d1 << 2) | (d2 >> 4));
						decoded[w++] = (byte)((d2 << 4) | (d3 >> 2));
					}
					break;
			}

			return w;
		}
		public byte[] Decode(in ReadOnlySpan<char> base64)
		{
			byte[] data = new byte[BytesNeeded(base64.Length)];
			Decode(base64, data);
			return data;
		}
	}
}
