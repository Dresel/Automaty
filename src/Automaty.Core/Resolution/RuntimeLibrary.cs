namespace Automaty.Core.Resolution
{
	using System.IO;

	public class RuntimeLibrary
	{
		public string AssemblyName => Path.GetFileNameWithoutExtension(FileName);

		public string FilePath => Path.Combine(DirectoryName, FileName);

		public string DirectoryName { get; set; }

		public string FileName { get; set; }

		public override string ToString()
		{
			return $"{AssemblyName}";
		}
	}
}