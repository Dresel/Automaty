namespace Automaty.Core.Resolution
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using NuGet.Versioning;

	// See https://github.com/OmniSharp/omnisharp-roslyn/blob/dev/src/OmniSharp.Abstractions/Services/DotNetInfo.cs
	public class DotNetInfo
	{
		private DotNetInfo()
		{
			IsEmpty = true;
		}

		private DotNetInfo(string version, string osName, string osVersion, string osPlatform, string rid, string basePath)
		{
			IsEmpty = false;

			Version = SemanticVersion.Parse(version);
			OSName = osName;
			OSVersion = osVersion;
			OSPlatform = osPlatform;
			RID = rid;
			BasePath = basePath;
		}

		public static DotNetInfo Empty { get; } = new DotNetInfo();

		public string BasePath { get; }

		public bool IsEmpty { get; }

		public string OSName { get; }

		public string OSPlatform { get; }

		public string OSVersion { get; }

		public string RID { get; }

		public SemanticVersion Version { get; }

		public static DotNetInfo GetInfo(string workingDirectory = null)
		{
			Process process;

			try
			{
				process = Start("--info", workingDirectory);
			}
			catch
			{
				return Empty;
			}

			List<string> lines = new List<string>();

			process.OutputDataReceived += (sender, e) =>
			{
				if (!string.IsNullOrWhiteSpace(e.Data))
				{
					lines.Add(e.Data);
				}
			};

			process.BeginOutputReadLine();
			process.WaitForExit();

			return Parse(lines);
		}

		public static DotNetInfo Parse(List<string> lines)
		{
			string version = string.Empty;
			string osName = string.Empty;
			string osVersion = string.Empty;
			string osPlatform = string.Empty;
			string rid = string.Empty;
			string basePath = string.Empty;

			foreach (string line in lines)
			{
				int colonIndex = line.IndexOf(':');
				if (colonIndex >= 0)
				{
					string name = line.Substring(0, colonIndex).Trim();
					string value = line.Substring(colonIndex + 1).Trim();

					if (name.Equals("Version", StringComparison.OrdinalIgnoreCase))
					{
						version = value;
					}
					else if (name.Equals("OS Name", StringComparison.OrdinalIgnoreCase))
					{
						osName = value;
					}
					else if (name.Equals("OS Version", StringComparison.OrdinalIgnoreCase))
					{
						osVersion = value;
					}
					else if (name.Equals("OS Platform", StringComparison.OrdinalIgnoreCase))
					{
						osPlatform = value;
					}
					else if (name.Equals("RID", StringComparison.OrdinalIgnoreCase))
					{
						rid = value;
					}
					else if (name.Equals("Base Path", StringComparison.OrdinalIgnoreCase))
					{
						basePath = value;
					}
				}
			}

			return new DotNetInfo(version, osName, osVersion, osPlatform, rid, basePath);
		}

		private static Process Start(string arguments, string workingDirectory)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("dotnet", arguments)
			{
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardOutput = true
			};

			return Process.Start(startInfo);
		}
	}
}