namespace Automaty.Core.Execution
{
	using System;
	using System.IO;
	using Automaty.Common.Logging;
	using Automaty.Common.Output;
	using Automaty.Core.Logging;
	using Automaty.Core.Output;

	public class ScriptContext : IScriptContext
	{
		public ScriptContext(string scriptFilePath, string projectFilePath) : this(scriptFilePath, projectFilePath,
			new NullLoggerFactory())
		{
		}

		public ScriptContext(string scriptFilePath, string projectFilePath, ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger<IScriptContext>();

			ScriptFilePath = Path.GetFullPath(scriptFilePath.ToPlatformSpecificPath());
			ProjectFilePath = string.IsNullOrEmpty(projectFilePath)
				? projectFilePath
				: Path.GetFullPath(projectFilePath.ToPlatformSpecificPath());

			Output = new FileCollectionWriter(ScriptFilePath, loggerFactory.CreateLogger<IFileCollectionWriter>())
			{
				CurrentFolder = Path.GetDirectoryName(ScriptFilePath)
			};
		}

		public ILogger<IScriptContext> Logger { get; set; }

		public IFileCollectionWriter Output { get; protected set; }

		public string ProjectFilePath { get; protected set; }

		public string ScriptFilePath { get; protected set; }

		protected bool Disposed { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}

			if (disposing)
			{
				Output.Dispose();
			}

			Disposed = true;
		}
	}
}