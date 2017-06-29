namespace Automaty.Core.Output
{
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using Automaty.Common.Output;

	public class FileWriter : IFileWriter
	{
		public FileWriter(string filePath)
		{
			TextWriter = File.CreateText(filePath);
		}

		public int IndentLevel { get; set; }

		public IOutputSettings OutputSettings { get; set; } = new OutputSettings();

		protected TextWriter TextWriter { get; }

		protected bool Disposed { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IIndent WithIndent()
		{
			return new Indent(this);
		}

		public IIndent WithIndent(int indentLevel)
		{
			return new Indent(this, indentLevel);
		}

		public IFileWriter WriteLine()
		{
			TextWriter.WriteLine();

			return this;
		}

		public IFileWriter WriteLine(string value)
		{
			TextWriter.WriteLine(value.Indent(IndentLevel, OutputSettings.IndentString));

			return this;
		}

		public Task<IFileWriter> WriteLineAsync()
		{
			return TextWriter.WriteLineAsync().ContinueWith(x => (IFileWriter)this);
		}

		public Task<IFileWriter> WriteLineAsync(string value)
		{
			return TextWriter.WriteLineAsync(value.Indent(IndentLevel, OutputSettings.IndentString))
				.ContinueWith(x => (IFileWriter)this);
		}

		public IIndent WriteScope()
		{
			return new Scope(this, string.Empty);
		}

		public IIndent WriteScope(string header)
		{
			return new Scope(this, header);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (Disposed)
			{
				return;
			}

			if (disposing)
			{
				TextWriter.Dispose();
			}

			Disposed = true;
		}
	}
}