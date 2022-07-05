namespace MichMcb.CsExt.Collections
{
	using System.Collections.Generic;
	/// <summary>
	/// Extension methods for various collections.
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Returns <paramref name="list"/> as a <see cref="IReadOnlyList{T}"/>. Does not copy the list.
		/// </summary>
		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
		{
			return list is List<T> realList
				? realList
				: list is T[] realArray
					? realArray
					: new ReadOnlyList<T>(list);
		}
		/// <summary>
		/// Returns <paramref name="collection"/> as a <see cref="IReadOnlyCollection{T}"/>. Does not copy the collection.
		/// </summary>
		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
		{
			return collection is List<T> realList
				? realList
				: collection is T[] realArray
					? realArray
					: new ReadOnlyCollection<T>(collection);
		}
		/// <summary>
		/// Returns <paramref name="dictionary"/> as a <see cref="IReadOnlyDictionary{TKey, TValue}"/>. Does not copy the dictionary.
		/// </summary>
		public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) where TKey : notnull
		{
			return dictionary is Dictionary<TKey, TValue> realDict
				? realDict
				: new ReadOnlyDictionary<TKey, TValue>(dictionary);
		}
#if NET5_0_OR_GREATER
		/// <summary>
		/// Returns <paramref name="set"/> as a <see cref="IReadOnlySet{T}"/>. Does not copy the set.
		/// </summary>
		public static IReadOnlySet<T> AsReadOnly<T>(this ISet<T> set)
		{
			return set is HashSet<T> realSet
				? realSet
				: new ReadOnlySet<T>(set);
		}
#endif
	}
}
