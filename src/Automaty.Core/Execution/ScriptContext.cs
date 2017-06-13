namespace Automaty.Core.Execution
{
	using System;
	using System.IO;
	using Automaty.Core.Logging;
	using Automaty.Core.Output;

	public class ScriptContext : IDisposable
	{
		public ScriptContext(string scriptFilePath, string projectFilePath) : this(scriptFilePath, projectFilePath,
			new NullLoggerFactory())
		{
		}

		public ScriptContext(string scriptFilePath, string projectFilePath, ILoggerFactory loggerFactory)
		{
			ScriptFilePath = scriptFilePath;
			ProjectFilePath = projectFilePath;

			Output = new FileCollectionWriter(loggerFactory.CreateLogger<FileCollectionWriter>())
			{
				CurrentFolder = Path.GetDirectoryName(scriptFilePath),
				DefaultFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(scriptFilePath),
					$"{Path.GetFileNameWithoutExtension(scriptFilePath)}.generated.cs")).ToPlatformSpecificPath()
			};
		}

		public FileCollectionWriter Output { get; protected set; }

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