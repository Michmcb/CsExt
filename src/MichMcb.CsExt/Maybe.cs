namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	/// <summary>
	/// A way of returning either a Value or an Error. Provides methods to safely get either the Value or Error.
	/// </summary>
	/// <typeparam name="TVal">The Type on success.</typeparam>
	/// <typeparam name="TErr">The Type on failure.</typeparam>
	public readonly struct Maybe<TVal, TErr>
	{
		private readonly TVal value;
		private readonly TErr error;
		/// <summary>
		/// Creates a new instance with <paramref name="value"/>.
		/// You shouln't need to use this often; this struct can be implicitly cast from objects of either <typeparamref name="TErr"/> or <typeparamref name="TVal"/>.
		/// </summary>
		/// <param name="value">The success value.</param>
		public Maybe(TVal value)
		{
			this.value = value;
			error = default!;
			Ok = true;
		}
		/// <summary>
		/// Creates a new instance with <paramref name="error"/>.
		/// You shouln't need to use this often; this struct can be implicitly cast from objects of either <typeparamref name="TErr"/> or <typeparamref name="TVal"/>.
		/// </summary>
		/// <param name="error">The error value.</param>
		public Maybe(TErr error)
		{
			value = default!;
			this.error = error;
			Ok = false;
		}
		/// <summary>
		/// If true, has a <typeparamref name="TVal"/>, otherwise has a <typeparamref name="TErr"/>.
		/// </summary>
		public bool Ok { get; }
		/// <summary>
		/// Gets the value, or <paramref name="ifNone"/> if <see cref="Ok"/> is false.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TVal ValueOr([AllowNull] TVal ifNone)
		{
			return Ok ? value : ifNone!;
		}
		/// <summary>
		/// Gets the value, or throws a <see cref="NoValueException"/>, passing the string representation of the error.
		/// </summary>
		public TVal ValueOrException()
		{
			return Ok ? value : throw new NoValueException(error?.ToString());
		}
		/// <summary>
		/// Gets the error, or <paramref name="ifNone"/> if <see cref="Ok"/> is true.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TErr ErrorOr([AllowNull] TErr ifNone)
		{
			return Ok ? ifNone! : error;
		}
		/// <summary>
		/// Returns the value of <see cref="Ok"/>. If true, then <paramref name="val"/> is set. Otherwise, <paramref name="error"/> is set.
		/// </summary>
		/// <param name="val">If <see cref="Ok"/> is true, the value. Otherwise, the default value for <typeparamref name="TVal"/>.</param>
		/// <param name="error">If <see cref="Ok"/> is false, the error. Otherwise, the default value for <typeparamref name="TErr"/>.</param>
		public bool Success([NotNullWhen(true)] out TVal val, [NotNullWhen(false)] out TErr error)
		{
			val = value;
			error = this.error;
			return Ok;
		}
		/// <summary>
		/// If <see cref="Ok"/> is true, sets <paramref name="val"/> to the Value for this instance and returns true.
		/// Otherwise, val is set to the default value for <typeparamref name="TVal"/> and returns false.
		/// </summary>
		public bool Success([NotNullWhen(true)] out TVal val)
		{
			val = value;
			return Ok;
		}
		/// <summary>
		/// Does the opposite of <see cref="Success(out TVal, out TErr)"/>.
		/// </summary>
		/// <param name="val">If <see cref="Ok"/> is true, the value. Otherwise, the default value for <typeparamref name="TVal"/>.</param>
		/// <param name="error">If <see cref="Ok"/> is false, the error. Otherwise, the default value for <typeparamref name="TErr"/>.</param>
		public bool Failure([NotNullWhen(false)] out TVal val, [NotNullWhen(true)] out TErr error)
		{
			val = value;
			error = this.error;
			return !Ok;
		}
		/// <summary>
		/// If <see cref="Ok"/> is false, sets <paramref name="error"/> to the Value for this instance and returns true.
		/// Otherwise, val is set to the default value for <typeparamref name="TErr"/> and returns false.
		/// </summary>
		public bool Failure([NotNullWhen(true)] out TErr error)
		{
			error = this.error;
			return !Ok;
		}
		/// <summary>
		/// Calls ToString() on the value if <see cref="Ok"/> is true, otherwise calls ToString() on the error.
		/// </summary>
		public override string ToString()
		{
			return Ok ? value?.ToString() ?? string.Empty : error?.ToString() ?? string.Empty;
		}
		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public override bool Equals(object? obj)
		{
			throw new InvalidOperationException("You cannot compare Maybe instances");
		}
		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public override int GetHashCode()
		{
			throw new InvalidOperationException("You cannot get HashCodes of Maybe instances");
		}
		/// <summary>
		/// Equivalent to new Maybe(<paramref name="value"/>, default, true);
		/// </summary>
		public static implicit operator Maybe<TVal, TErr>([DisallowNull] TVal value) => new(value);
		/// <summary>
		/// Equivalent to new Maybe(default, <paramref name="error"/>, true);
		/// </summary>
		public static implicit operator Maybe<TVal, TErr>([DisallowNull] TErr error) => new(error);
		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static bool operator ==(Maybe<TVal, TErr> left, Maybe<TVal, TErr> right) => throw new InvalidOperationException("You cannot compare Maybe instances");
		/// <summary>
		/// Throws <see cref="InvalidOperationException"/>.
		/// </summary>
		/// <exception cref="InvalidOperationException"/>
		public static bool operator !=(Maybe<TVal, TErr> left, Maybe<TVal, TErr> right) => throw new InvalidOperationException("You cannot compare Maybe instances");
		/// <summary>
		/// Equivalent to calling the constructor and passing <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		public static Maybe<TVal, TErr> Value(TVal value)
		{
			return new(value);
		}
		/// <summary>
		/// Equivalent to calling the constructor and passing <paramref name="err"/>.
		/// </summary>
		/// <param name="err">The error.</param>
		public static Maybe<TVal, TErr> Error(TErr err)
		{
			return new(err);
		}
	}
}
