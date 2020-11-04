﻿namespace MichMcb.CsExt
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// A way of returning either a Value or an Error. Provides methods to safely get either the Value or Error.
	/// You can also use this type directly if an if statement; it evaluates to true/false based on the value of <see cref="Ok"/>.
	/// If <see cref="Ok"/> is true, then only <typeparamref name="TVal"/> is valid.
	/// If <see cref="Ok"/> is false, then only <typeparamref name="TErr"/> is valid.
	/// </summary>
	/// <typeparam name="TVal">The Type on success.</typeparam>
	/// <typeparam name="TErr">The Type on failure.</typeparam>
	public readonly struct Maybe<TVal, TErr>
	{
		private readonly TVal value;
		private readonly TErr error;
		/// <summary>
		/// Creates a new instance with <paramref name="value"/>, and the value of <paramref name="ok"/>.
		/// You shouln't need to use this often; this struct can be implicitly cast from objects of either <typeparamref name="TErr"/> or <typeparamref name="TVal"/>.
		/// </summary>
		/// <param name="value">The success value.</param>
		/// <param name="ok">If true, success. If false, failure.</param>
		public Maybe(TVal value, bool ok = true)
		{
			this.value = value;
			error = default!;
			Ok = ok;
		}
		/// <summary>
		/// Creates a new instance with <paramref name="value"/>, and the value of <paramref name="error"/>.
		/// You shouln't need to use this often; this struct can be implicitly cast from objects of either <typeparamref name="TErr"/> or <typeparamref name="TVal"/>.
		/// </summary>
		/// <param name="error">The error value.</param>
		/// <param name="ok">If true, success. If false, failure.</param>
		public Maybe(TErr error, bool ok = false)
		{
			value = default!;
			this.error = error;
			Ok = ok;
		}
		/// <summary>
		/// If true, has a <typeparamref name="TVal"/>, otherwise has a <typeparamref name="TErr"/>.
		/// When this instance is used in an If statement, it produces this value.
		/// </summary>
		public bool Ok { get; }
		/// <summary>
		/// Gets the value, or <paramref name="ifNone"/> if <see cref="Ok"/> is false.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TVal ValueOr([AllowNull] TVal ifNone) => Ok ? value : ifNone;
		/// <summary>
		/// Gets the error, or <paramref name="ifNone"/> if <see cref="Ok"/> is true.
		/// </summary>
		[return: NotNullIfNotNull("ifNone")]
		public TErr ErrorOr([AllowNull] TErr ifNone) => Ok ? ifNone : error;
#pragma warning disable CS8762 // Parameter must have a non-null value when exiting in some condition.
		/// <summary>
		/// If <see cref="Ok"/> is true, sets <paramref name="val"/> to the Value for this instance and returns true.
		/// Otherwise, val is set to the default value for <typeparamref name="TVal"/> and returns false.
		/// </summary>
		public bool HasValue([NotNullWhen(true)] out TVal val)
		{
			val = value;
			return Ok;
		}
		/// <summary>
		/// If <see cref="Ok"/> is false, sets <paramref name="error"/> to the Value for this instance and returns true.
		/// Otherwise, val is set to the default value for <typeparamref name="TErr"/> and returns false.
		/// </summary>
		public bool HasError([NotNullWhen(true)] out TErr error)
		{
			error = this.error;
			return !Ok;
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
#pragma warning restore CS8762 // Parameter must have a non-null value when exiting in some condition.
		/// <summary>
		/// Equivalent to new Maybe(<paramref name="value"/>, default, true);
		/// </summary>
		public static implicit operator Maybe<TVal, TErr>([DisallowNull] TVal value)
		{
			return new Maybe<TVal, TErr>(value, true);
		}
		/// <summary>
		/// Equivalent to new Maybe(default, <paramref name="error"/>, true);
		/// </summary>
		public static implicit operator Maybe<TVal, TErr>([DisallowNull] TErr error)
		{
			return new Maybe<TVal, TErr>(error, false);
		}
		/// <summary>
		/// Calls ToString() on the value if <see cref="Ok"/> is true, otherwise calls ToString() on the error.
		/// </summary>
		public override string ToString()
		{
			return Ok ? value?.ToString() ?? string.Empty : error?.ToString() ?? string.Empty;
		}
		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("You cannot compare Maybe instances");
		}
		public override int GetHashCode()
		{
			throw new InvalidOperationException("You cannot get HashCodes of Maybe instances");
		}
	}
}
