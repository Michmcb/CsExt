namespace MichMcb.CsExt.Collections
{
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Wraps a <see cref="ICollection{T}"/>, to make it a <see cref="IReadOnlyCollection{T}"/>.
	/// </summary>
	public sealed class ReadOnlyCollection<T> : IReadOnlyCollection<T>
	{
		private readonly ICollection<T> collection;
		/// <summary>
		/// Wraps <see cref="collection"/>.
		/// </summary>
		/// <param name="collection">The collection to wrap.</param>
		public ReadOnlyCollection(ICollection<T> collection)
		{
			this.collection = collection;
		}
		/// <inheritdoc/>
		public int Count => collection.Count;
		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			return collection.GetEnumerator();
		}
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return collection.GetEnumerator();
		}
	}
}
