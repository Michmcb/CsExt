namespace MichMcb.CsExt.Rng
{
	using System;
	using System.Buffers.Binary;

	/// <summary>
	/// Wraps <see cref="Random"/>.
	/// </summary>
	public sealed class RandomRng : IRng
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="instance">The <see cref="Random"/> instance to use.</param>
		public RandomRng(Random instance)
		{
			Instance = instance;
		}
		/// <summary>
		/// The instance being used.
		/// </summary>
		public Random Instance { get; }
		/// <inheritdoc/>
		public void FillBytes(Span<byte> span)
		{
#if NET6_0_OR_GREATER
			Instance.NextBytes(span);
#else
			byte[] buf = new byte[span.Length];
			Instance.NextBytes(buf);
			buf.CopyTo(span);
#endif
		}
		/// <inheritdoc/>
		public byte[] NextBytes(int count)
		{
			byte[] buf = new byte[count];
			FillBytes(buf);
			return buf;
		}
		/// <inheritdoc/>
		public double NextDouble()
		{
			return Instance.NextDouble();
		}
		/// <inheritdoc/>
		public int NextInt32()
		{
			return Instance.Next();
		}
		/// <inheritdoc/>
		public int NextInt32(int minValue, int maxValue)
		{
			return Instance.Next(minValue, maxValue);
		}
		/// <inheritdoc/>
		public uint NextUInt32()
		{
			Span<byte> buf = stackalloc byte[4];
			FillBytes(buf);
			return BinaryPrimitives.ReadUInt32LittleEndian(buf);
		}
		/// <inheritdoc/>
		public uint NextUInt32(uint minValue, uint maxValue)
		{
			return minValue <= maxValue
				? (uint)(((ulong)maxValue - minValue) * NextDouble()) + minValue
				: throw new ArgumentOutOfRangeException(nameof(minValue), "minValue is larger than maxValue");
		}
	}
}
