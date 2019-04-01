namespace Automaty.Core.Resolution
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Automaty.Common.Logging;
	using Automaty.Core.Logging;
	using Microsoft.Build.Evaluation;
	using Microsoft.Build.Locator;
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

			projectFilePath = Path.GetFullPath(projectFilePath.ToPlatformSpecificPath());

			RegisterDotNetSdk();

			//Environment.SetEnvironmentVariable(MSBuildFinder.MSBuildEnvironmentVariableName, MSBuildFinder.Find());

			ProjectCollection projectCollection = new ProjectCollection();
			Project project = projectCollection.LoadProject(projectFilePath);

			List<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

			runtimeLibraries.AddRange(GetProjectReferences(projectFilePath, projectCollection, project));
			runtimeLibraries.AddRange(GetNugetReferences(projectFilePath, project));

			return runtimeLibraries;
		}

		protected string GetMultiTargetingProjectAssemblyPath(string directoryPath, string targetFrameworks,
			string outputPath, string assemblyName)
		{
			string targetFramework = GetSuitableTargetFramework(targetFrameworks);
			Logger.WriteDebug($"Using target framework {targetFramework}.");

			return Path.GetFullPath(Path.Combine(directoryPath, outputPath, targetFramework, $"{assemblyName}.dll"));
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
				Logger.WriteError(
					$"Lock file {lockFileFilePath} not found. Run dotnet restore before executing Automaty.");

				throw new AutomatyException();
			}

			LockFile lockFile = lockFileFormat.Read(lockFileFilePath);

			string targetFramework = project.GetPropertyValue(PropertyNames.TargetFramework);

			if (string.IsNullOrEmpty(targetFramework))
			{
				Logger.WriteDebug("Multi targeting project assembly detected.");
				targetFramework = GetSuitableTargetFramework(project.GetPropertyValue(PropertyNames.TargetFrameworks));
				Logger.WriteDebug($"Using target framework {targetFramework}.");
			}

			LockFileTarget lockFileTarget =
				lockFile.GetTarget(NuGetUtils.ParseFrameworkName(targetFramework), string.Empty);

			NuGetPackageResolver nuGetPackageResolver =
				NuGetPackageResolver.CreateResolver(lockFile, Path.GetDirectoryName(projectFilePath));

			// Add netstandard / netcoreapp placeholder libraries,
			// because not every assembly will be loaded with Assembly.Load(new AssemblyName("netstandard"))
			List<LockFileTargetLibrary> placeholderLibraries = lockFileTarget.Libraries.Where(library =>
				library.Name.StartsWith("System.") &&
				library.CompileTimeAssemblies.Any(file => NuGetUtils.IsPlaceholderFile(file.Path))).ToList();

			foreach (LockFileTargetLibrary library in placeholderLibraries)
			{
				Logger.WriteDebug($"Adding netstandard / netcoreapp placeholder library \"{library.Name}\".");

				runtimeLibraries.Add(new RuntimeLibrary
				{
					Name = library.Name,
					FileName = $"{library.Name}.dll",
					DirectoryName = "_._",
					IsPlaceholder = true,
				});
			}

			// Add nuget references
			foreach (LockFileTargetLibrary library in lockFileTarget.Libraries)
			{
				if (library.Type != LibraryType.Package)
				{
					continue;
				}

				string packageDirectory = nuGetPackageResolver.GetPackageDirectory(library.Name, library.Version);

				foreach (LockFileItem file in library.CompileTimeAssemblies.Where(file =>
					!NuGetUtils.IsPlaceholderFile(file.Path)))
				{
					string filePath = Path.GetFullPath(Path.Combine(packageDirectory, file.Path));

					Logger.WriteDebug($"Adding \"{filePath}\".");

					runtimeLibraries.Add(new RuntimeLibrary
					{
						Name = library.Name,
						DirectoryName = Path.GetDirectoryName(filePath),
						FileName = Path.GetFileName(filePath)
					});
				}
			}

			return runtimeLibraries;
		}

		protected string GetProjectAssemblyPath(string directoryPath, string outputPath, string assemblyName)
		{
			return Path.GetFullPath(Path.Combine(directoryPath, outputPath, $"{assemblyName}.dll"));
		}

		protected IEnumerable<RuntimeLibrary> GetProjectReferences(string projectFilePath,
			ProjectCollection projectCollection, Project project)
		{
			Logger.WriteInfo("Adding project references.");

			ICollection<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

			ICollection<ProjectItem> projectItems = project.GetItems(ItemNames.ProjectReference);

			// Add project references
			foreach (ProjectItem projectItem in projectItems)
			{
				Project referencedProject =
					projectCollection.LoadProject(Path.Combine(Path.GetDirectoryName(projectFilePath),
						projectItem.EvaluatedInclude));

				// TODO: Log?
				if (referencedProject.GetProperty(PropertyNames.OutputType).EvaluatedValue != "Library")
				{
					continue;
				}

				string outputPath = referencedProject.GetProperty(PropertyNames.OutputPath).EvaluatedValue
					.ToPlatformSpecificPath();
				string assemblyName = referencedProject.GetProperty(PropertyNames.AssemblyName).EvaluatedValue;
				string targetFrameworks = referencedProject.GetProperty(PropertyNames.TargetFrameworks)?.EvaluatedValue;

				string filePath;
				if (targetFrameworks != null)
				{
					Logger.WriteDebug("Multi targeting project assembly referenced.");
					filePath = GetMultiTargetingProjectAssemblyPath(referencedProject.DirectoryPath, targetFrameworks,
						outputPath, assemblyName);
				}
				else
				{
					filePath = GetProjectAssemblyPath(referencedProject.DirectoryPath, outputPath, assemblyName);
				}

				Logger.WriteDebug($"Adding \"{filePath}\".");

				runtimeLibraries.Add(new RuntimeLibrary
				{
					Name = assemblyName,
					DirectoryName = Path.GetDirectoryName(filePath),
					FileName = Path.GetFileName(filePath)
				});
			}

			return runtimeLibraries;
		}

		protected string GetSuitableTargetFramework(string targetFrameworks)
		{
			string[] frameworks = targetFrameworks.Split(';');

			string targetFramework =
				frameworks.FirstOrDefault(f => f.StartsWith("netstandard", StringComparison.OrdinalIgnoreCase)) ??
				frameworks.FirstOrDefault(f => f.StartsWith("netcoreapp", StringComparison.OrdinalIgnoreCase)) ??
				frameworks[0];

			return targetFramework;
		}

		protected void RegisterDotNetSdk()
		{
			MSBuildLocator.RegisterInstance(MSBuildLocator.QueryVisualStudioInstances(
				new VisualStudioInstanceQueryOptions()
				{
					DiscoveryTypes = DiscoveryType.DotNetSdk,
				}).SingleOrDefault());
		}
	}
}