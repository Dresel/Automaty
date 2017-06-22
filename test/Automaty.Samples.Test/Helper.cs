namespace Automaty.Samples.Test
{
	using System;
	using System.Diagnostics;
	using System.IO;

	public class Helper
	{
		public static void AssertGeneratedFileDoesNotExist(string sampleProjectDirectoryPath, string generatedFilePath)
		{
			if (File.Exists(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)))
			{
				throw new InvalidOperationException("Generated file already exist.");
			}
		}

		public static void AssertGeneratedFileExists(string sampleProjectDirectoryPath, string generatedFilePath)
		{
			if (!File.Exists(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)))
			{
				throw new InvalidOperationException("Generated file does not exist.");
			}
		}

		public static void AssertSampleProjectDirectoryPathExists(string projectDirectoryPath)
		{
			if (!Directory.Exists(projectDirectoryPath))
			{
				throw new InvalidOperationException("Sample directory directory does not exist.");
			}
		}

		public static void AutomatyRun(string sampleProjectDirectoryPath, string parameter)
		{
			if (StartProcess(sampleProjectDirectoryPath, "dotnet", $"automaty run {parameter} --verbose") != 0)
			{
				throw new InvalidOperationException("dotnet automaty failed.");
			}
		}

		public static void DotNetBuild(string sampleProjectDirectoryPath, string projectFilePath)
		{
			if (StartProcess(sampleProjectDirectoryPath, "dotnet", $"build {projectFilePath}") != 0)
			{
				throw new InvalidOperationException("dotnet build failed.");
			}
		}

		public static void DotNetRestore(string sampleProjectDirectoryPath, string projectFilePath)
		{
			if (StartProcess(sampleProjectDirectoryPath, "dotnet", $"restore {projectFilePath}") != 0)
			{
				throw new InvalidOperationException("dotnet restore failed.");
			}
		}

		public static int StartProcess(string workingDirectory, string fileName, string arguments)
		{
			Process process = Process.Start(new ProcessStartInfo
			{
				WorkingDirectory = workingDirectory,
				FileName = fileName,
				Arguments = arguments,
				RedirectStandardOutput = true,
				RedirectStandardError = true
			});

			process.OutputDataReceived += (sender, args) => { Console.WriteLine(args.Data); };
			process.ErrorDataReceived += (sender, args) => { Console.Error.WriteLine(args.Data); };

			process.BeginOutputReadLine();
			process.BeginErrorReadLine();
			process.WaitForExit();

			return process.ExitCode;
		}
	}
}