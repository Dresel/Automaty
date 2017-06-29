namespace Automaty.Core
{
	using System;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;

	public static class StringExtension
	{
		public static string Indent(this string value, int indentLevel, string indentString)
		{
			if (indentLevel == 0 || string.IsNullOrEmpty(indentString))
			{
				return value;
			}

			string indentStringRepeated = new StringBuilder(indentString.Length * indentLevel)
				.Insert(0, indentString, indentLevel).ToString();

			if (!(value.StartsWith("\r\n") || value.StartsWith("\r") || value.StartsWith("\n")))
			{
				value = indentStringRepeated + value;
			}

			return Regex.Replace(value, @"(\r\n|\r|\n)(?![\r\n|\r|\n])", match => match.Value + indentStringRepeated);
		}

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