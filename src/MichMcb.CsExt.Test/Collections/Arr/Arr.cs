//namespace MichMcb.CsExt.Test.Collections.Arr
//{
//	using Xunit;
//	using MichMcb.CsExt.Collections;

//	public sealed class Tests
//	{
//		[Fact]
//		public void TestIteration()
//		{
//			Arr<int> ints = new Arr<int>(5) { 1, 2, 3, 4, 5 };
//			int x = 1;
//			foreach (int i in ints)
//			{
//				Assert.Equal(x, i);
//				x++;
//			}
//			using Arr<int>.Enumerator e = ints.GetEnumerator();
//			x = 1;
//			while (e.MoveNext())
//			{
//				Assert.Equal(x, e.Current);
//				x++;
//			}
//		}
//		[Fact]
//		public void Adding()
//		{
//			Arr<int> ints = new(5);
//			ints.Add(1);
//			ints.Add(2);
//			ints.Add(3);
//			ints.Add(4);
//			ints.Add(5);
//		}
//	}
//}
