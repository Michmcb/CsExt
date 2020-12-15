//namespace MichMcb.CsExt.Collections
//{
//	using System;
//	using System.Collections;
//	using System.Collections.Generic;
//	using System.Linq;
//	using System.Runtime.CompilerServices;
//	using SysArray = System.Array;

//	/// <summary>
//	/// A wrapper around an Array.
//	/// Acts similarly to a <see cref="List{T}"/>, but it allows you direct access to the array as well.
//	/// It also allows iteration while modifying the collection, but beware that resizing the collection to be smaller may cause errors.
//	/// </summary>
//	/// <typeparam name="T">The type of the elements.</typeparam>
//	public class Arr<T> : IList<T>, IReadOnlyCollection<T>
//	{
//		private T[] _array;
//		/// <summary>
//		/// Creates a new instance with a new empty array as the underlying storage.
//		/// </summary>
//		public Arr()
//		{
//			_array = new T[4];
//		}
//		/// <summary>
//		/// Creates a new instance with a new array of size <paramref name="capacity"/> as the underlying storage.
//		/// </summary>
//		/// <param name="capacity">The length of the array.</param>
//		public Arr(int capacity)
//		{
//			_array = new T[capacity];
//		}
//		/// <summary>
//		/// Creates a new instance with a new array, containing the elements copied from <paramref name="collection"/>.
//		/// </summary>
//		/// <param name="collection">The elements to copy into a new array.</param>
//		public Arr(ICollection<T> collection)
//		{
//			_array = new T[collection.Count];
//			collection.CopyTo(_array, 0);
//		}
//		/// <summary>
//		/// Creates a new instance with a new array, containing the elements copied from <paramref name="collection"/>.
//		/// </summary>
//		/// <param name="collection">The elements to copy into a new array.</param>
//		public Arr(IEnumerable<T> collection)
//		{
//			if (collection is ICollection<T> typed)
//			{
//				_array = new T[typed.Count];
//				typed.CopyTo(_array, 0);
//			}
//			else
//			{
//				_array = collection.ToArray();
//			}
//		}
//		/// <summary>
//		/// Creates a new instance, and either takes ownership of <paramref name="array"/>, or copies it, depending on the value of <paramref name="copy"/>.
//		/// </summary>
//		/// <param name="copy">If <paramref name="copy"/> is true, copies elements. Otherwise, uses <paramref name="array"/> directly.</param>
//		/// <param name="array">The array.</param>
//		public Arr(bool copy, params T[] array)
//		{
//			if (copy)
//			{
//				_array = new T[array.Length];
//				if (array.Length != 0)
//				{
//					SysArray.Copy(array, _array, array.Length);
//				}
//			}
//			else
//			{
//				_array = array;
//			}
//		}
//		/// <summary>
//		/// Gets or sets the <typeparamref name="T"/> at <paramref name="index"/>.
//		/// </summary>
//		/// <param name="index">The index.</param>
//		/// <returns>The <typeparamref name="T"/> at <paramref name="index"/>.</returns>
//		public T this[int index] { get => _array[index]; set => _array[index] = value; }
//		/// <summary>
//		/// The underlying array used for storage.
//		/// To resize the array, set the <see cref="Capacity"/> property. Don't use <see cref="System.Array.Resize{T}(ref T[], int)"/> on this directly.
//		/// Setting this will reset <see cref="Count"/> to 0.
//		/// Anything in this array whose index is equal to or greater than <see cref="Count"/> is not considered used yet and will probably be null/default.
//		/// </summary>
//		public T[] Array
//		{
//			get => _array;
//			set
//			{
//				_array = value;
//				Count = 0;
//			}
//		}
//		/// <summary>
//		/// The number of items added to the array.
//		/// Setting this will change the index at which objects are added (see <see cref="Add(T)"/>).
//		/// </summary>
//		public int Count { get; set; }
//		/// <summary>
//		/// Returns the Length of the array.
//		/// Setting this value will resize the array.
//		/// If this is set to a smaller value than <see cref="Count"/>, then <see cref="Count"/> will be set to the same value as Capacity.
//		/// </summary>
//		public int Capacity
//		{
//			get => _array.Length;
//			set
//			{
//				// Don't do anything if it's set to the exact same value
//				if (_array.Length != value)
//				{
//					// Resize throws exception if value < 0
//					SysArray.Resize(ref _array, value);
//					if (value < Count)
//					{
//						Count = value;
//					}
//				}
//			}
//		}
//		/// <summary>
//		/// Returns false.
//		/// </summary>
//		public bool IsReadOnly => false;
//		/// <summary>
//		/// Adds <paramref name="item"/> to the index specified by <see cref="Count"/>, and increments <see cref="Count"/> by 1.
//		/// </summary>
//		/// <param name="item">The item to add.</param>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public void Add(T item)
//		{
//			int c = Count + 1;
//			EnsureCapacity(c);
//			_array[c] = item;
//			Count = c;
//		}
//		/// <summary>
//		/// Returns the used slice of <see cref="Array"/> as a <see cref="Span{T}"/>.
//		/// The slice ranges from 0 to <see cref="Count"/>.
//		/// </summary>
//		/// <returns>The slice of the array that's used as a span.</returns>
//		public Span<T> AsSpan()
//		{
//			return _array.AsSpan(0, Count);
//		}
//		/// <summary>
//		/// Returns the used slice of <see cref="Array"/> as a <see cref="Memory{T}"/>.
//		/// The slice ranges from 0 to <see cref="Count"/>.
//		/// </summary>
//		/// <returns>The slice of the array that's used as a span.</returns>
//		public Memory<T> AsMemory()
//		{
//			return _array.AsMemory(0, Count);
//		}
//		/// <summary> 
//		/// Clears the array and sets <see cref="Count"/> to 0.
//		/// </summary>
//		[MethodImpl(MethodImplOptions.AggressiveInlining)]
//		public void Clear()
//		{
//			SysArray.Clear(_array, 0, _array.Length);
//			Count = 0;
//		}
//		/// <summary>
//		/// Returns true if <paramref name="item"/> is found in the array.
//		/// </summary>
//		/// <param name="item">The item to find.</param>
//		/// <returns>True if <paramref name="item"/> was found, false otherwise.</returns>
//		public bool Contains(T item)
//		{
//			return IndexOf(item) != -1;
//		}
//		/// <summary>
//		/// Copies the contents of the array to the provided <paramref name="array"/>, starting at <paramref name="arrayIndex"/>.
//		/// </summary>
//		/// <param name="array">The array to copy to.</param>
//		/// <param name="arrayIndex">The index at which to start copying.</param>
//		public void CopyTo(T[] array, int arrayIndex)
//		{
//			// This throws all the exceptions we need
//			SysArray.Copy(_array, array, arrayIndex);
//		}
//		/// <summary>
//		/// If <see cref="Array"/> is smaller than <paramref name="capacity"/>, resizes <see cref="Array"/> so it has space for at least <paramref name="capacity"/> elements.
//		/// Does nothing if <see cref="Array"/> already has sufficient capacity.
//		/// </summary>
//		/// <param name="capacity">Minimum capacity</param>
//		public void EnsureCapacity(int capacity)
//		{
//			if (Array.Length < capacity)
//			{
//				// The const below is taken from .NET source, I mean if it's exceeded, what on earth are you DOING using an array this big!
//				Capacity = (int)Math.Min(Array.Length == 0 ? 4L : Array.Length * 2L, 0x7fefffffL);
//			}
//		}
//		/// <summary>
//		/// Returns an enumerator to loop all elements at the time of invocation of this.
//		/// Any new elements added after calling this method will not be looped over.
//		/// </summary>
//		public Enumerator GetEnumerator()
//		{
//			return new Enumerator(this, Count);
//		}
//		IEnumerator<T> IEnumerable<T>.GetEnumerator()
//		{
//			return new Enumerator(this, Count);
//		}
//		IEnumerator IEnumerable.GetEnumerator()
//		{
//			return new Enumerator(this, Count);
//		}
//		/// <summary>
//		/// Returns the index of <paramref name="item"/> in the array, or -1 if it is not found.
//		/// </summary>
//		/// <param name="item">The item to find.</param>
//		/// <returns>The index of <paramref name="item"/> if found, or -1 if not found.</returns>
//		public int IndexOf(T item)
//		{
//			return SysArray.IndexOf(_array, item);
//		}
//		/// <summary>
//		/// Inserts the item into the array at <paramref name="index"/>, and moves the elements past that up by 1.
//		/// Increases <see cref="Count"/> by 1.
//		/// </summary>
//		/// <param name="index">The index at which to insert the item.</param>
//		/// <param name="item">The item to insert.</param>
//		public void Insert(int index, T item)
//		{
//			int c = Count + 1;
//			EnsureCapacity(c);
//			// We only have to shift everything upwards if the item is inserted before other valid elements
//			// Say if we have 5 items (0, 1, 2, 3, 4) and we insert at index 5, then we have to shift nothing!
//			if (index < Count)
//			{
//				SysArray.Copy(_array, index, _array, index + 1, Count - index);
//			}
//			_array[index] = item;
//			Count = c;
//		}
//		/// <summary>
//		/// Removes the first instance of <paramref name="item"/> from the array.
//		/// </summary>
//		/// <param name="item">The item to remove.</param>
//		/// <returns>True if the item was found and removed, false otherwise.</returns>
//		public bool Remove(T item)
//		{
//			int i = IndexOf(item);
//			if (i != -1)
//			{
//				RemoveAt(i);
//				return true;
//			}
//			return false;
//		}
//		/// <summary>
//		/// Removes the item from the array at <paramref name="index"/>, and moves the elements past that down by 1.
//		/// Decreases <see cref="Count"/> by 1.
//		/// </summary>
//		/// <param name="index">The index from which to remove the item.</param>
//		public void RemoveAt(int index)
//		{
//			// TODO Only copy downwards in the same sort of situation that Insert does so
//			SysArray.Copy(_array, index + 1, _array, index, Count - index);
//			--Count;
//		}
//		/// <summary>
//		/// Enumerates an <see cref="Arr{T}"/>.
//		/// </summary>
//		public struct Enumerator : IEnumerator<T>, IEnumerator
//		{
//			private readonly Arr<T> a;
//			private int index;
//			private int length;
//			private T cur;
//			/// <summary>
//			/// Creates a new enumerator, starting at 0 and iterating over <paramref name="length"/> elements.
//			/// </summary>
//			/// <param name="a">The <see cref="Arr{T}"/> to enumerate over.</param>
//			/// <param name="length">The number of elements to iterate over.</param>
//			public Enumerator(Arr<T> a, int length)
//			{
//				this.a = a;
//				index = 0;
//				this.length = length;
//				cur = default!;
//			}
//			/// <summary>
//			/// The current <typeparamref name="T"/>.
//			/// </summary>
//			public T Current => cur;
//			/// <summary>
//			/// The current object.
//			/// </summary>
//			object IEnumerator.Current => cur!;
//			/// <summary>
//			/// Does nothing.
//			/// </summary>
//			public void Dispose() { }
//			/// <summary>
//			/// Moves to the next value.
//			/// </summary>
//			/// <returns>True if there was another element, false otherwise.</returns>
//			public bool MoveNext()
//			{
//				if (index < length)
//				{
//					cur = a._array[index];
//					index++;
//					return true;
//				}
//				return false;
//			}
//			/// <summary>
//			/// Resets iteration to the beginning of the collection.
//			/// </summary>
//			public void Reset()
//			{
//				index = 0;
//				length = a.Count;
//				cur = default!;
//			}
//		}
//	}
//}