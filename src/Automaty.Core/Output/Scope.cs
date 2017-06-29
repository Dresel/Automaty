namespace Automaty.Core.Output
{
	using System;
	using Automaty.Common.Output;

	public class Scope : IIndent
	{
		public Scope(FileWriter fileWriter, string header) : this(fileWriter, header, fileWriter.IndentLevel + 1)
		{
		}

		public Scope(FileWriter fileWriter, string header, int indentLevel)
		{
			FileWriter = fileWriter;
			IndentLevel = FileWriter.IndentLevel;

			if (!string.IsNullOrEmpty(header))
			{
				FileWriter.WriteLine(header);
			}

			FileWriter.WriteLine("{");

			FileWriter.IndentLevel = indentLevel;
		}

		protected FileWriter FileWriter { get; set; }

		protected int IndentLevel { get; set; }

		private bool Disposed { get; set; }

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
				FileWriter.IndentLevel = IndentLevel;
				FileWriter.WriteLine("}");
			}

			Disposed = true;
		}
	}
}