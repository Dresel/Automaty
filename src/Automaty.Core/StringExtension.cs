namespace Automaty.Core
{
	using System;
	using System.IO;

	public static class StringExtension
	{
		public static string ToPlatformSpecificPath(this string path)
		{
			switch (Path.DirectorySeparatorChar)
			{
				case '/': return path.Replace('\\', '/');

				case '\\': return path.Replace('/', '\\');

				default: throw new InvalidOperationException();
			}
		}
	}
}