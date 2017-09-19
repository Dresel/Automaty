namespace Automaty.Core.Resolution
{
	// See https://github.com/OmniSharp/omnisharp-roslyn/blob/dev/src/OmniSharp.MSBuild/ProjectFile/ProjectFileInfo.PropertyNames.cs
	internal static class PropertyNames
	{
		public const string AssemblyName = nameof(PropertyNames.AssemblyName);
		public const string OutputPath = nameof(PropertyNames.OutputPath);
		public const string OutputType = nameof(PropertyNames.OutputType);
		public const string ProjectAssetsFile = nameof(PropertyNames.ProjectAssetsFile);
		public const string TargetFramework = nameof(PropertyNames.TargetFramework);
		public const string TargetFrameworks = nameof(PropertyNames.TargetFrameworks);
	}
}