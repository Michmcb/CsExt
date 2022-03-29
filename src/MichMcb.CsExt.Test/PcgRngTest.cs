namespace MichMcb.CsExt.Test
{
	using MichMcb.CsExt.Rng;
	using System;
	using Xunit;

	public static class PcgRngTest
	{
		[Fact]
		public static void DoesntCrashIGuess()
		{
			PcgRng rng1 = new();
			rng1.NextInt32();
		}
		[Fact]
		public static void InitialSeed()
		{
			PcgRng rng1 = new(10, 123);
			PcgRng rng2 = new(10, 123);
			Assert.Equal(rng1.NextInt32(), rng2.NextInt32());
		}
		[Fact]
		public static void Range()
		{
			PcgRng rng = new();
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
		[Fact]
		public static void Stuff()
		{
			// return (int)(((long)NextUInt32((uint)(minValue + 2147483648), (uint)(maxValue + 2147483648))) - 2147483648);
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
				var xd = uint.MaxValue / UIntMaxDouble;
			}
		}
	}
}
