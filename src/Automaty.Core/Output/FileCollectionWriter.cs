namespace Automaty.Core.Output
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Automaty.Core.Logging;

	public class FileCollectionWriter : IDisposable
	{
		public FileCollectionWriter() : this(new NullLogger<FileCollectionWriter>())
		{
		}

		public FileCollectionWriter(ILogger<FileCollectionWriter> logger)
		{
			Logger = logger;
		}

		public FileWriter Default => this[DefaultFilePath];

		public Dictionary<string, FileWriter> Files { get; } = new Dictionary<string, FileWriter>();

		public string CurrentFolder { get; set; }

		public string DefaultFilePath { get; set; }

		public ILogger<FileCollectionWriter> Logger { get; set; }

		public OutputSettings Settings { get; set; } = new OutputSettings();

		protected bool Disposed { get; set; }

		public FileWriter this[string filePath]
		{
			get
			{
				if (Disposed)
				{
					throw new ObjectDisposedException(nameof(FileCollectionWriter));
				}

				filePath = Path.Combine(CurrentFolder, filePath);

				if (!Files.TryGetValue(filePath, out FileWriter fileWriter))
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