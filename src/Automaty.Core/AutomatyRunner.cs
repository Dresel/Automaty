namespace Automaty.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Automaty.Common.Logging;
	using Automaty.Core.Execution;
	using Automaty.Core.Logging;
	using Automaty.Core.Resolution;

#if NETSTANDARD1_6
	using System.Runtime.Loader;
#else
	using System.Reflection;
#endif

	public class AutomatyRunner
	{
		public AutomatyRunner() : this(new NullLoggerFactory())
		{
		}

		public AutomatyRunner(ILoggerFactory loggerFactory)
		{
			LoggerFactory = loggerFactory;
		}

		public ILoggerFactory LoggerFactory { get; set; }

		public bool Execute(IEnumerable<string> sourceFilePaths, string projectFilePath)
		{
			ILogger<AutomatyRunner> logger = LoggerFactory.CreateLogger<AutomatyRunner>();

			try
			{
				if (sourceFilePaths == null || !sourceFilePaths.Any())
				{
					logger.WriteWarning($"{nameof(sourceFilePaths)} is null or empty.");
					return true;
				}

				IEnumerable<RuntimeLibrary> runtimeLibraries = new List<RuntimeLibrary>();

				if (!string.IsNullOrEmpty(projectFilePath))
				{
					if (!File.Exists(projectFilePath))
					{
						logger.WriteError($"Project file path '{projectFilePath}' not found.");
						return true;
					}

					RuntimeLibraryResolver runtimeLibraryResolver =
						new RuntimeLibraryResolver(LoggerFactory.CreateLogger<RuntimeLibraryResolver>());

					runtimeLibraries = runtimeLibraryResolver.GetRuntimeLibraries(projectFilePath);

#if NETSTANDARD1_6
					AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
					{
						RuntimeLibrary runtimeLibrary = runtimeLibraries.FirstOrDefault(x => x.AssemblyName == assemblyName.Name);

						if (runtimeLibrary != null)
						{
							logger.WriteDebug($"Resolving for {runtimeLibrary.AssemblyName}.");
						}

						return runtimeLibrary == null ? null : assemblyLoadContext.LoadFromAssemblyPath(runtimeLibrary.FilePath);
					};
#else
					AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
					{
						RuntimeLibrary runtimeLibrary = runtimeLibraries.FirstOrDefault(x => x.AssemblyName == args.Name);

						if (runtimeLibrary != null)
						{
							logger.WriteDebug($"Resolving for {runtimeLibrary.AssemblyName}.");
						}

						return runtimeLibrary == null ? null : Assembly.LoadFile(runtimeLibrary.FilePath);
					};
#endif
				}

				ScriptCompiler scriptCompiler = new ScriptCompiler(LoggerFactory.CreateLogger<ScriptCompiler>());
				scriptCompiler.AddRuntimeLibraries(runtimeLibraries);

				ScriptRunner scriptRunner = new ScriptRunner(scriptCompiler, LoggerFactory);

				foreach (string sourceFilePath in sourceFilePaths)
				{
					if (!File.Exists(sourceFilePath))
					{
						logger.WriteError($"Source file path '{sourceFilePath}' not found.");
						continue;
					}

					scriptRunner.Run(sourceFilePath);
				}
			}
			catch (AutomatyException automatyException)
			{
				logger.WriteError($"Exception of type {typeof(AutomatyException)} occured: {automatyException}.");

				return true;
			}
			catch (Exception e)
			{
				logger.WriteError($"Exception of type {e.GetType()} occured: {e}.");

				return false;
			}

			return false;
		}
	}
}