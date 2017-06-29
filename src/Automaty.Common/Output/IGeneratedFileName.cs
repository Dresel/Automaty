namespace Automaty.Common.Output
{
	public interface IGeneratedFileName
	{
		string CombinedFileName { get; }

		string ScriptFileNameWithoutExtension { get; }

		bool AddGeneratedAsSuffix { get; set; }

		bool AddScriptFileNameAsPrefix { get; set; }

		string Extension { get; set; }

		string FileNameWithoutExtension { get; set; }
	}
}