namespace MichMcb.CsExt.Data
{
	using System;
	using System.IO;

	/// <summary>
	/// Calculates the CRC32 hash of data that's written to or read from it.
	/// Seeking this stream will reset its CRC32.
	/// </summary>
	[Obsolete("Use Crc32.NET instead, it's much faster/better")]
	public sealed class Crc32Stream : Stream
	{
		private uint crc32w;
		private uint crc32r;
		/// <summary>
		/// Creates a new instance which writes/reads <paramref name="stream"/>.
		/// </summary>
		/// <param name="stream">The stream to write/read</param>
		public Crc32Stream(Stream stream)
		{
			crc32w = Crc32.InitialValue;
			crc32r = Crc32.InitialValue;
			Stream = stream;
		}
		/// <summary>
		/// If <see cref="Stream"/> can read.
		/// </summary>
		public override bool CanRead => Stream.CanRead;
		/// <summary>
		/// If <see cref="Stream"/> can seek.
		/// </summary>
		public override bool CanSeek => Stream.CanSeek;
		/// <summary>
		/// If <see cref="Stream"/> can write.
		/// </summary>
		public override bool CanWrite => Stream.CanWrite;
		/// <summary>
		/// Length of <see cref="Stream"/>.
		/// </summary>
		public override long Length => Stream.Length;
		/// <summary>
		/// Gets/Sets Position of <see cref="Stream"/>. This will reset both CRC32s.
		/// </summary>
		public override long Position
		{
			get => Stream.Position;
			set
			{
				crc32r = Crc32.InitialValue;
				crc32w = Crc32.InitialValue;
				Stream.Position = value;
			}
		}
		/// <summary>
		/// The underlying stream.
		/// </summary>
		public Stream Stream { get; }
		/// <summary>
		/// The CRC32 calculated from reads
		/// </summary>
		public uint Crc32Read => ~crc32r;
		/// <summary>
		/// The CRC32 calculated from writes
		/// </summary>
		public uint Crc32Write => ~crc32w;
		/// <summary>
		/// Flushes <see cref="Stream"/>
		/// </summary>
		public override void Flush()
		{
			Stream.Flush();
		}
		/// <summary>
		/// Reads from <see cref="Stream"/>. Updates <see cref="Crc32Read"/> after the read.
		/// </summary>
		/// <param name="buffer">Buffer to fill.</param>
		/// <param name="offset">Offset at which to start storing bytes.</param>
		/// <param name="count">Max number of bytes to read from <see cref="Stream"/>.</param>
		/// <returns>Number of bytes read.</returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			int read = Stream.Read(buffer, offset, count);
			crc32r = Crc32.GetCrc32(buffer, crc32r, offset, read);
			return read;
		}
		/// <summary>
		/// Writes to <see cref="Stream"/>. Updates <see cref="Crc32Write"/> after the read.
		/// </summary>
		/// <param name="buffer">Buffer to write.</param>
		/// <param name="offset">Offset at which to start writing bytes.</param>
		/// <param name="count">Number of bytes to write to <see cref="Stream"/>.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			Stream.Write(buffer, offset, count);
			crc32w = Crc32.GetCrc32(buffer, crc32w, offset, count);
		}
		/// <summary>
		/// Seeks <see cref="Stream"/>. Resets the CRC32s to their initial values by doing this.
		/// </summary>
		/// <param name="offset">Byte offset relative to <paramref name="origin"/>.</param>
		/// <param name="origin">Reference point to obtain new position</param>
		/// <returns>New position in <see cref="Stream"/>.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			crc32r = Crc32.InitialValue;
			crc32w = Crc32.InitialValue;
			return Stream.Seek(offset, origin);
		}
		/// <summary>
		/// Seeks <see cref="Stream"/>. This does not reset the CRC32s.
		/// </summary>
		/// <param name="offset">Byte offset relative to <paramref name="origin"/>.</param>
		/// <param name="origin">Reference point to obtain new position</param>
		/// <returns>New position in <see cref="Stream"/>.</returns>
		public long SeekPreserveCrc32(long offset, SeekOrigin origin)
		{
			return Stream.Seek(offset, origin);
		}
		/// <summary>
		/// Sets the length of <see cref="Stream"/>.
		/// </summary>
		/// <param name="value">The length to set <see cref="Stream"/> to.</param>
		public override void SetLength(long value)
		{
			Stream.SetLength(value);
		}
	}
}
