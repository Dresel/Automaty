namespace Automaty.EFCoreSample.MSBuild
{
	public static class StringExtensions
	{
		public static string FirstCharacterToLower(this string text)
		{
			if (string.IsNullOrEmpty(text) || char.IsLower(text, 0))
			{
				return text;
			}

			return char.ToLowerInvariant(text[0]) + text.Substring(1);
		}
	}
}