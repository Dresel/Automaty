namespace Automaty.MSBuild
{
	using System.Diagnostics;
	using System.Linq;
	using Microsoft.Build.Framework;
	using Microsoft.Build.Utilities;

	public class AutomatyTask : Task
	{
		public bool IsVerbose { get; set; } = false;

		public string ProjectFilePath { get; set; }

		[Required]
		public ITaskItem[] ScriptFiles { get; set; }

		public override bool Execute()
		{
			// Can't use Automaty directly, see
			// https://stackoverflow.com/questions/44434564/
			// https://github.com/Microsoft/msbuild/issues/2195
			// https://github.com/dotnet/roslyn/issues/20134

			if (ScriptFiles == null || !ScriptFiles.Any())
			{
				Log.LogWarning("ScriptFiles is null or empty.");
				return true;
			}

			// Instead we call the DotNetCli tool
			string arguments = $"automaty run {string.Join(" ", ScriptFiles.Select(x => $"\"{x.ItemSpec}\""))}";

			if (string.IsNullOrEmpty(ProjectFilePath))
			{
				Log.LogWarning("ProjectFilePath is null or empty.");
			}
			else
			{
				arguments += $" --project \"{ProjectFilePath}\"";
			}

			if (IsVerbose)
			{
				arguments += " --verbose";
			}

			ProcessStartInfo projectStartInfo = new ProcessStartInfo
			{
				FileName = "dotnet",
				Arguments = arguments,
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			};

			Process process = new Process
			{
				StartInfo = projectStartInfo
			};

			process.OutputDataReceived += (sender, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.Data))
				{
					Log.LogMessage(MessageImportance.Normal, e.Data);
				}
			};

			process.ErrorDataReceived += (sender, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.Data))
				{
					Log.LogError(e.Data);
				}
			};

			process.Start();
			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			return true;
		}
	}
}