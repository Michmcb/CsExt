using System;

namespace MichMcb.CsExt
{
	/// <summary>
	/// Returned when no exception actually happened.
	/// To return this, use the static Inst or InstAsBase property.
	/// </summary>
#pragma warning disable CA1032 // Implement standard exception constructors
	public class NoException : Exception
#pragma warning restore CA1032 // Implement standard exception constructors
	{
		/// <summary>
		/// The singleton instance
		/// </summary>
		public static NoException Inst { get; } = new NoException();
		/// <summary>
		/// Returns Inst, typed as Exception.
		/// </summary>
		public static Exception InstAsBase => Inst;
		/// <summary>
		/// You cannot create instances of this class; instead, use NoException.Inst
		/// </summary>
		private NoException() { }
	}
}
