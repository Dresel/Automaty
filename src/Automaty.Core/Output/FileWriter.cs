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

		public IFileWriter WriteLine(string line)
		{
			TextWriter.WriteLine(line);

			return this;
		}

		public async Task<IFileWriter> WriteLineAsync(string line)
		{
			await TextWriter.WriteLineAsync(line);

			return this;
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