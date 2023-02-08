namespace MichMcb.CsExt.Test
{
	using MichMcb.CsExt.Rng;
	using System;
	using Xunit;
	public static class RngTest
	{
		private static readonly IRng[] rngs = new IRng[] { new PcgRng(), new LcgRng(), new RandomRng(Random.Shared) };
		[Fact]
		public static void DoesntCrashIGuess()
		{
			foreach (var rng in rngs)
			{
				rng.NextInt32();
			}
		}
		[Fact]
		public static void InitialSeed()
		{
			Check(new PcgRng(10, 123), new PcgRng(10, 123));
			Check(new LcgRng(15), new LcgRng(15));
			Check(new RandomRng(new(123)), new RandomRng(new(123)));

			static void Check(IRng rng1, IRng rng2)
			{
				Assert.Equal(rng1.NextInt32(), rng2.NextInt32());
				Assert.Equal(rng1.NextUInt32(), rng2.NextUInt32());
				Assert.Equal(rng1.NextDouble(), rng2.NextDouble());
				Assert.Equal(rng1.NextBytes(16), rng2.NextBytes(16));
			}
		}
		[Fact]
		public static void Range()
		{
			foreach (var rng in rngs)
			{
				for (int i = 0; i < 10_000_000; i++)
				{
					int val = (int)rng.NextUInt32(10, 20);
					Assert.InRange(val, 10, 19);
				}
				for (int i = 0; i < 10_000_000; i++)
				{
					int val = rng.NextInt32(10, 20);
					Assert.InRange(val, 10, 19);
				}
				Assert.True(10 == rng.NextUInt32(10, 10));
				Assert.Equal(10, rng.NextInt32(10, 10));
				Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextUInt32(10, 0));
				Assert.Throws<ArgumentOutOfRangeException>(() => rng.NextInt32(10, 0));
			}
		}
		[Fact]
		public static void Stuff()
		{
			{
				uint x = (uint)(int.MinValue + 2147483648);
				Assert.True(x == uint.MinValue);

				x = int.MaxValue + 2147483648;
				Assert.True(x == uint.MaxValue);
			}
			{
				long val = uint.MaxValue;
				var x = val - 2147483648;
				val = 0;
				var y = val - 2147483648;

				Assert.Equal(int.MaxValue, x);
				Assert.Equal(int.MinValue, y);
			}
			{
				double UIntMaxDouble = uint.MaxValue + 1d;
				Assert.Equal(0, 0 / UIntMaxDouble);
				_ = uint.MaxValue / UIntMaxDouble;
			}
		}
	}
}
