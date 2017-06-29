namespace Automaty.Core.Output
{
	using System.IO;
	using Automaty.Common.Output;

	public class GeneratedFileName : IGeneratedFileName
	{
		public GeneratedFileName(string scriptFilePath)
		{
			ScriptFileNameWithoutExtension = Path.GetFileNameWithoutExtension(scriptFilePath);
		}

		public string CombinedFileName
		{
			get
			{
				string combinedFileName = string.Empty;

				if (AddScriptFileNameAsPrefix)
				{
					combinedFileName += $"{ScriptFileNameWithoutExtension}.";
				}

				if (!string.IsNullOrEmpty(FileNameWithoutExtension))
				{
					combinedFileName += $"{FileNameWithoutExtension}.";
				}

				if (AddGeneratedAsSuffix)
				{
					combinedFileName += "generated.";
				}

				return $"{combinedFileName}{Extension}";
			}
		}

		public string ScriptFileNameWithoutExtension { get; }

		public bool AddGeneratedAsSuffix { get; set; } = true;

		public bool AddScriptFileNameAsPrefix { get; set; } = true;

		public string Extension { get; set; } = "cs";

		public string FileNameWithoutExtension { get; set; }
	}
}