namespace Automaty.Core.Resolution
{
	using System.IO;

	// Inspired by https://github.com/OmniSharp/omnisharp-roslyn/blob/dev/src/OmniSharp.MSBuild/MSBuildEnvironment.cs
	public class MSBuildFinder
	{
		internal const string MSBuildEnvironmentVariableName = "MSBUILD_EXE_PATH";

		public static string Find()
		{
			return GetMSBuildExePath(DotNetInfo.GetInfo().BasePath);
		}

		private static string GetMSBuildExePath(string basePath)
		{
			// Look for MSBuild.exe or MSBuild.dll.
			string msbuildExePath = Path.Combine(basePath, "MSBuild.exe");

			if (File.Exists(msbuildExePath))
			{
				return msbuildExePath;
			}

			msbuildExePath = Path.Combine(basePath, "MSBuild.dll");

			if (File.Exists(msbuildExePath))
			{
				return msbuildExePath;
			}

			return null;
		}
	}
}