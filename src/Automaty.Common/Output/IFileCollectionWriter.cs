namespace Automaty.Common.Output
{
	using System;
	using System.Collections.Generic;
	using Automaty.Common.Logging;

	public interface IFileCollectionWriter : IDisposable
	{
		IFileWriter Default { get; }

		IDictionary<string, IFileWriter> Files { get; }

		string CurrentFolder { get; set; }

		string DefaultFilePath { get; set; }

		ILogger<IFileCollectionWriter> Logger { get; set; }

		IOutputSettings Settings { get; set; }

		IFileWriter this[string filePath] { get; }
	}
}