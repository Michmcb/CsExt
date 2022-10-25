namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// A way of indicating a value or absence of a value. Provides a method to safely get the value.
	/// This can also be useful with deserializing JSON, to distinguish between not passing a property, passing null, or passing a value.
	/// </summary>
	/// <typeparam name="TVal">The type of the value.</typeparam>
	public readonly struct Opt<TVal>
	{
		private readonly TVal val;
		/// <summary>
		/// Creates a new instance with the provided value.
		/// </summary>
		/// <param name="val">The value.</param>
		public Opt(TVal val)
		{
			this.val = val;
			Ok = true;
		}
		/// <summary>
		/// Returns true if a value is present, false otherwise.
		/// </summary>
		/// <param name="value">If a value is present, the value. Otherwise, the default value for <typeparamref name="TVal"/>.</param>
		/// <returns>true if a value is present, false otherwise.</returns>
		public bool HasVal([NotNullWhen(true)] out TVal value)
		{
			value = val;
			return Ok;
		}
		/// <summary>
		/// True if a value is present, false otherwise.
		/// </summary>
		public bool Ok { get; }
		/// <summary>
		/// Returns the value if one is present, otherwise returns <paramref name="ifNone"/>.
		/// </summary>
		/// <param name="ifNone">The default value.</param>
		/// <returns>The value or <paramref name="ifNone"/>.</returns>
		[return: NotNullIfNotNull("ifNone")]
		public TVal ValueOrDefault(TVal ifNone)
		{
			return Ok ? val : ifNone;
		}
		/// <summary>
		/// Gets the value, or throws a <see cref="NoValueException"/>.
		/// </summary>
		[Obsolete("Prefer the overloads taking a message or a func")]
		public TVal ValueOrException()
		{
			return Ok ? val : throw new NoValueException("Opt instance had no value.");
		}
		/// <summary>
		/// Gets the value, or throws a <see cref="NoValueException"/> with the provided <paramref name="message"/>.
		/// </summary>
		/// <param name="message">The message to set on the <see cref="NoValueException"/>.</param>
		public TVal ValueOrException(string message)
		{
			return Ok ? val : throw new NoValueException(message);
		}
		/// <summary>
		/// Gets the value, or throws the exception returned by <paramref name="getException"/>.
		/// </summary>
		/// <param name="getException">Creates an exception to throw.</param>
		public TVal ValueOrException(Func<Exception> getException)
		{
			return Ok ? val : throw getException();
		}
		/// <summary>
		/// Equivalent to creating a new instance.
		/// </summary>
		/// <param name="val">The value.</param>
		public static implicit operator Opt<TVal>(TVal val) => new(val);
	}
}