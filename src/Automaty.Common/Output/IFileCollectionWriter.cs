namespace Automaty.Common.Output
{
	using System;
	using System.Collections.Generic;

	public interface IFileCollectionWriter : IDisposable
	{
		IFileWriter Current { get; }

		IGeneratedFileName CurrentGeneratedFileName { get; }

		IDictionary<string, IFileWriter> Files { get; }

		string CurrentFolder { get; set; }

		IOutputSettings Settings { get; set; }

		IFileWriter this[string filePath] { get; }
	}
}