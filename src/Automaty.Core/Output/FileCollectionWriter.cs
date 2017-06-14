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
		public FileCollectionWriter() : this(new NullLogger<IFileCollectionWriter>())
		{
		}

		public FileCollectionWriter(ILogger<IFileCollectionWriter> logger)
		{
			Logger = logger;
		}

		public IFileWriter Default => this[DefaultFilePath];

		public IDictionary<string, IFileWriter> Files { get; } = new Dictionary<string, IFileWriter>();

		public string CurrentFolder { get; set; }

		public string DefaultFilePath { get; set; }

		public ILogger<IFileCollectionWriter> Logger { get; set; }

		public IOutputSettings Settings { get; set; } = new OutputSettings();

		protected bool Disposed { get; set; }

		public IFileWriter this[string filePath]
		{
			get
			{
				if (Disposed)
				{
					throw new ObjectDisposedException(nameof(FileCollectionWriter));
				}

				filePath = Path.Combine(CurrentFolder, filePath);

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