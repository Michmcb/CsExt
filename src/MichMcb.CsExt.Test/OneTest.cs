namespace MichMcb.CsExt.Test
{
	using MichMcb.CsExt;
	using System;
	using Xunit;

	public static class OneTest
	{
		[Fact]
		public static void Test()
		{
			One<int> o = new(50);
			Assert.Equal(50, o.Value);
			Assert.Equal(50, o[0]);
			Assert.Throws<IndexOutOfRangeException>(() => o[1]);
#pragma warning disable xUnit2013 // Do not use equality check to check for collection size.
			Assert.Equal(1, o.Count);
#pragma warning restore xUnit2013 // Do not use equality check to check for collection size.
			Assert.Collection(o, x => Assert.Equal(50, x));

			{
				System.Collections.IEnumerator enumerable = ((System.Collections.IEnumerable)o).GetEnumerator();
				Assert.True(enumerable.MoveNext());
				Assert.Equal(50, enumerable.Current);
				Assert.False(enumerable.MoveNext());
				Assert.Equal(0, enumerable.Current);

				enumerable.Reset();
				Assert.True(enumerable.MoveNext());
				Assert.Equal(50, enumerable.Current);
				Assert.False(enumerable.MoveNext());
				Assert.Equal(0, enumerable.Current);
			}
			{
				System.Collections.Generic.IEnumerator<int> enumerable = o.GetEnumerator();
				Assert.True(enumerable.MoveNext());
				Assert.Equal(50, enumerable.Current);
				Assert.False(enumerable.MoveNext());
				Assert.Equal(0, enumerable.Current);

				enumerable.Reset();
				Assert.True(enumerable.MoveNext());
				Assert.Equal(50, enumerable.Current);
				Assert.False(enumerable.MoveNext());
				Assert.Equal(0, enumerable.Current);
			}
		}
	}
}
