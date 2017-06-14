namespace Automaty.Core.Resolution
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Automaty.Common.Logging;
	using Automaty.Core.Logging;
	using Microsoft.Build.Evaluation;
	using NuGet.LibraryModel;
	using NuGet.ProjectModel;

	public class RuntimeLibraryResolver
	{
		public RuntimeLibraryResolver() : this(new NullLogger<RuntimeLibraryResolver>())
		{
		}

		public RuntimeLibraryResolver(ILogger<RuntimeLibraryResolver> logger)
		{
			Logger = logger;
		}

		public ILogger<RuntimeLibraryResolver> Logger { get; set; }

		public IEnumerable<RuntimeLibrary> GetRuntimeLibraries(string projectFilePath)
		{
			Logger.WriteDebug("Reading project file.");

			Environment.SetEnvironmentVariable(MSBuildFinder.MSBuildEnvironmentVariableName, MSBuildFinder.Find());

			ProjectCollection projectCollection = new ProjectCollection();
			Project project = projectCollection.LoadProject(projectFilePath);

			List<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

			runtimeLibraries.AddRange(GetProjectReferences(projectFilePath, projectCollection, project));
			runtimeLibraries.AddRange(GetNugetReferences(projectFilePath, project));

			return runtimeLibraries;
		}

		protected IEnumerable<RuntimeLibrary> GetNugetReferences(string projectFilePath, Project project)
		{
			Logger.WriteInfo("Adding nuget references.");

			ICollection<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

			LockFileFormat lockFileFormat = new LockFileFormat();

			string lockFileFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath),
				project.GetPropertyValue(PropertyNames.ProjectAssetsFile));

			if (!File.Exists(lockFileFilePath))
			{
				Logger.WriteError($"Lock file {lockFileFilePath} not found. Run dotnet restore before executing Automaty.");

				throw new AutomatyException();
			}

			LockFile lockFile = lockFileFormat.Read(lockFileFilePath);
			LockFileTarget lockFileTarget = lockFile.GetTarget(
				NuGetUtils.ParseFrameworkName(project.GetPropertyValue(PropertyNames.TargetFramework)), string.Empty);

			NuGetPackageResolver nuGetPackageResolver =
				NuGetPackageResolver.CreateResolver(lockFile, Path.GetDirectoryName(projectFilePath));

			// Add nuget references
			foreach (LockFileTargetLibrary library in lockFileTarget.Libraries)
			{
				if (library.Type != LibraryType.Package)
				{
					continue;
				}

				string packageDirectory = nuGetPackageResolver.GetPackageDirectory(library.Name, library.Version);

				foreach (LockFileItem file in library.RuntimeAssemblies.Where(file => !NuGetUtils.IsPlaceholderFile(file.Path)))
				{
					string filePath = Path.GetFullPath(Path.Combine(packageDirectory, file.Path));

					Logger.WriteDebug($"Adding \"{filePath}\".");

					runtimeLibraries.Add(new RuntimeLibrary
					{
						DirectoryName = Path.GetDirectoryName(filePath),
						FileName = Path.GetFileName(filePath)
					});
				}
			}

			return runtimeLibraries;
		}

		protected IEnumerable<RuntimeLibrary> GetProjectReferences(string projectFilePath,
			ProjectCollection projectCollection, Project project)
		{
			Logger.WriteInfo("Adding project references.");

			ICollection<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

			string outputPath = project.GetProperty(PropertyNames.OutputPath).EvaluatedValue;
			ICollection<ProjectItem> projectItems = project.GetItems(ItemNames.ProjectReference);

			// Add project references
			foreach (ProjectItem projectItem in projectItems)
			{
				Project referencedProject =
					projectCollection.LoadProject(Path.Combine(Path.GetDirectoryName(projectFilePath), projectItem.EvaluatedInclude));

				// TODO: Log?
				if (referencedProject.GetProperty(PropertyNames.OutputType).EvaluatedValue != "Library")
				{
					continue;
				}

				string assemblyName = referencedProject.GetProperty(PropertyNames.AssemblyName).EvaluatedValue;

				string filePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFilePath), outputPath,
					$"{assemblyName}.dll"));

				Logger.WriteDebug($"Adding \"{filePath}\".");

				runtimeLibraries.Add(new RuntimeLibrary
				{
					DirectoryName = Path.GetDirectoryName(filePath),
					FileName = Path.GetFileName(filePath)
				});
			}

			return runtimeLibraries;
		}
	}
}