namespace MichMcb.CsExt.Collections
{
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Wraps a <see cref="IList{T}"/>, to make it a <see cref="IReadOnlyList{T}"/>.
	/// </summary>
	public sealed class ReadOnlyList<T> : IReadOnlyList<T>
	{
		private readonly IList<T> list;
		/// <summary>
		/// Wraps <paramref name="list"/>.
		/// </summary>
		/// <param name="list">The list to wrap</param>
		public ReadOnlyList(IList<T> list)
		{
			this.list = list;
		}
		/// <inheritdoc/>
		public T this[int index] => list[index];
		/// <inheritdoc/>
		public int Count => list.Count;
		/// <inheritdoc/>
		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}
		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}
	}
}
