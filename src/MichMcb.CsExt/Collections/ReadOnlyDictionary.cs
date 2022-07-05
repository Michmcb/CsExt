namespace MichMcb.CsExt.Collections
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Wraps a <see cref="IDictionary{TKey, TValue}"/>, to make it a <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
	/// </summary>
	public sealed class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> where TKey : notnull
	{
		private readonly IDictionary<TKey, TValue> dictionary;
		/// <summary>
		/// Wraps <see cref="dictionary"/>
		/// </summary>
		/// <param name="dictionary">The dictionary to wrap.</param>
		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			this.dictionary = dictionary;
		}
		/// <inheritdoc/>
		public TValue this[TKey key] => dictionary[key];
		/// <inheritdoc/>
		public IEnumerable<TKey> Keys => dictionary.Keys;
		/// <inheritdoc/>
		public IEnumerable<TValue> Values => dictionary.Values;
		/// <inheritdoc/>
		public int Count => dictionary.Count;
		/// <inheritdoc/>
		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}
		/// <inheritdoc/>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
		/// <inheritdoc/>
		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}
	}
}
