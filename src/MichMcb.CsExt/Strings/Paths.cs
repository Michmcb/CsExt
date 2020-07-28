using System;

namespace MichMcb.CsExt.Strings
{
	public readonly struct PathFragments
	{
		public PathFragments(Range directory, Range fileName, Range fileNameWithoutExtension, Range extension)
		{
			Directory = directory;
			FileName = fileName;
			FileNameWithoutExtension = fileNameWithoutExtension;
			Extension = extension;
		}
		/// <summary>
		/// The directory, with trailing PathSeparatorChar
		/// </summary>
		public Range Directory { get; }
		/// <summary>
		/// The filename, with extension
		/// </summary>
		public Range FileName { get; }
		public Range FileNameWithoutExtension { get; }
		/// <summary>
		/// The extension, with the leading dot
		/// </summary>
		public Range Extension { get; }
	}
	public static class Paths
	{
		public static PathFragments GetFragments(in ReadOnlySpan<char> path)
		{
			throw new NotImplementedException("");
		}
	}
}
