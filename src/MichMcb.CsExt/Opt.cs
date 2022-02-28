namespace MichMcb.CsExt
{
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// A way of indicating a value or absence of a value. Provides a method to safely get the value.
	/// </summary>
	/// <typeparam name="TVal">The type of the value.</typeparam>
	public readonly struct Opt<TVal>
	{
		private readonly TVal val;
		private readonly bool hasVal;
		/// <summary>
		/// Creates a new instance with the provided value.
		/// </summary>
		/// <param name="val">The value.</param>
		public Opt(TVal val)
		{
			this.val = val;
			hasVal = true;
		}
		/// <summary>
		/// Returns true if a value is present, false otherwise.
		/// </summary>
		/// <param name="value">If a value is present, the value. Otherwise, the default value for <typeparamref name="TVal"/>.</param>
		/// <returns>true if a value is present, false otherwise.</returns>
		public bool HasVal([NotNullWhen(true)] out TVal value)
		{
			value = val;
			return hasVal;
		}
	}
}