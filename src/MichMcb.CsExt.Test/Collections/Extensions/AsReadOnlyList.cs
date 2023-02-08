namespace MichMcb.CsExt.Test.Collections.Extensions
{
	using MichMcb.CsExt.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public static class AsReadOnly
	{
		public static void List()
		{
			IList<int> list = new List<int>() { 1, 2, 3, 4, 5 };
			CheckList(list, Extensions.AsReadOnly(list));
			CheckList(list, new ReadOnlyList<int>(list));

			list = new int[] { 5, 4, 3, 2, 1 };
			CheckList(list, Extensions.AsReadOnly(list));
			CheckList(list, new ReadOnlyList<int>(list));
		}
		private static void CheckList(IList<int> expected, IReadOnlyList<int> actual)
		{
			Assert.Equal(expected.Count, actual.Count);
			for (int i = 0; i < expected.Count; i++)
			{
				Assert.Equal(expected[i], actual[i]);
			}
			using IEnumerator<int> e1 = expected.GetEnumerator();
			using IEnumerator<int> e2 = actual.GetEnumerator();
			bool go = true;
			while (go)
			{
				Assert.Equal(go = e1.MoveNext(), e2.MoveNext());
				Assert.Equal(e1.Current, e2.Current);
			}
		}
		public static void Collection()
		{
			ICollection<int> collection = new List<int>() { 1, 2, 3, 4, 5 };
			CheckCollection(collection, collection.AsReadOnly());
			CheckCollection(collection, new ReadOnlyCollection<int>(collection));

			collection = new int[] { 5, 4, 3, 2, 1 };
			CheckCollection(collection, collection.AsReadOnly());
			CheckCollection(collection, new ReadOnlyCollection<int>(collection));
		}
		private static void CheckCollection(ICollection<int> expected, IReadOnlyCollection<int> actual)
		{
			Assert.Equal(expected.Count, actual.Count);
			using IEnumerator<int> e1 = expected.GetEnumerator();
			using IEnumerator<int> e2 = actual.GetEnumerator();
			bool go = true;
			while (go)
			{
				Assert.Equal(go = e1.MoveNext(), e2.MoveNext());
				Assert.Equal(e1.Current, e2.Current);
			}
		}
		public static void Dictionary()
		{
			IDictionary<int, int> dict = new Dictionary<int, int>()
			{
				[1] = 1,
				[2] = 2,
				[3] = 3,
				[4] = 4,
				[5] = 5,
			};
			CheckDict(dict, Extensions.AsReadOnly(dict));
			CheckDict(dict, new ReadOnlyDictionary<int, int>(dict));
		}
		private static void CheckDict(IDictionary<int, int> expected, IReadOnlyDictionary<int, int> actual)
		{
			Assert.Equal(expected.Count, actual.Count);

			foreach (KeyValuePair<int, int> kvp in expected)
			{
				Assert.True(actual.ContainsKey(kvp.Key));
				Assert.True(actual.TryGetValue(kvp.Key, out int value));
				Assert.Equal(kvp.Value, value);
			}
			foreach (int key in expected.Keys)
			{
				Assert.True(actual.ContainsKey(key));
				Assert.True(actual.TryGetValue(key, out _));
			}

			List<int> expectedValues = expected.Values.ToList();
			List<int> actualValues = actual.Values.ToList();
			expectedValues.Sort();
			actualValues.Sort();
			Assert.Equal(expectedValues.Count, actualValues.Count);
			for (int i = 0; i < expectedValues.Count; i++)
			{
				Assert.Equal(expectedValues[i], actualValues[i]);
			}
		}
		public static void Set()
		{
			ISet<int> set = new HashSet<int>() { 1, 2, 3, 4, 5, };
			CheckSet(set, set.AsReadOnly());
			CheckSet(set, new ReadOnlySet<int>(set));
		}
		private static void CheckSet(ISet<int> expected, IReadOnlySet<int> actual)
		{
			Assert.Equal(expected.Count, actual.Count);
			foreach (int item in expected)
			{
				Assert.True(actual.Contains(item));
			}
			Assert.True(expected.SetEquals(actual));
			Assert.True(actual.SetEquals(expected));
			Assert.True(expected.Overlaps(actual));
			Assert.True(actual.Overlaps(expected));
		}
	}
}
