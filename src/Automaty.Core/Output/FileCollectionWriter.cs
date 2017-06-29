namespace Automaty.Core.Output
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Automaty.Common.Logging;
	using Automaty.Common.Output;
	using Automaty.Core.Logging;

	public class FileCollectionWriter : IFileCollectionWriter
	{
		public FileCollectionWriter(string scriptFilePath) : this(scriptFilePath, new NullLogger<IFileCollectionWriter>())
		{
		}

		public FileCollectionWriter(string scriptFilePath, ILogger<IFileCollectionWriter> logger)
		{
			Logger = logger;
			CurrentGeneratedFileName = new GeneratedFileName(scriptFilePath);
		}

		public IFileWriter Current => this[CurrentGeneratedFileName.CombinedFileName];

		public IGeneratedFileName CurrentGeneratedFileName { get; }

		public IDictionary<string, IFileWriter> Files { get; } = new Dictionary<string, IFileWriter>();

		public string CurrentFolder { get; set; }

		public IOutputSettings Settings { get; set; } = new OutputSettings();

		protected bool Disposed { get; set; }

		protected ILogger<IFileCollectionWriter> Logger { get; set; }

		public IFileWriter this[string filePath]
		{
			get
			{
				if (Disposed)
				{
					throw new ObjectDisposedException(nameof(FileCollectionWriter));
				}

				filePath = Path.GetFullPath(Path.Combine(CurrentFolder.ToPlatformSpecificPath(),
					filePath.ToPlatformSpecificPath()));

				if (!Files.TryGetValue(filePath, out IFileWriter fileWriter))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));

					Logger.WriteInfo($"Writing to {filePath}.");

					fileWriter = new FileWriter(filePath)
					{
						OutputSettings = new OutputSettings
						{
							IndentString = Settings.IndentString
						}
					};

					Files.Add(filePath, fileWriter);
				}

				return fileWriter;
			}
		}

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
				foreach (FileWriter fileWriter in Files.Values)
				{
					fileWriter.Dispose();
				}
			}

			Disposed = true;
		}
	}
}