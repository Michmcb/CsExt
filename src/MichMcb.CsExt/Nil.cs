namespace MichMcb.CsExt
{
	/// <summary>
	/// Can be used instead of void to represent no return type.
	/// </summary>
	public readonly struct Nil
	{
		/// <summary>
		/// An instance.
		/// It's the same as new Nil();
		/// </summary>
		public static readonly Nil Inst = new();
		/// <summary>
		/// Always returns false
		/// </summary>
		/// <returns>false</returns>
		public override bool Equals(object? obj)
		{
			return false;
		}
		/// <summary>
		/// Always returns 0
		/// </summary>
		/// <returns>0</returns>
		public override int GetHashCode()
		{
			return 0;
		}
		/// <summary>
		/// Always returns string.Empty
		/// </summary>
		/// <returns>string.Empty</returns>
		public override string ToString()
		{
			return string.Empty;
		}
#pragma warning disable IDE0060 // Remove unused parameter
		/// <summary>
		/// Always returns true.
		/// </summary>
		/// <returns>true</returns>
		public static bool operator ==(Nil left, Nil right) => true;
		/// <summary>
		/// Always returns false.
		/// </summary>
		/// <returns>false</returns>
		public static bool operator !=(Nil left, Nil right) => false;
#pragma warning restore IDE0060 // Remove unused parameter
	}
}
