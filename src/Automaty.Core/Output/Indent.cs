namespace Automaty.Core.Output
{
	using System;
	using Automaty.Common.Output;

	public class Indent : IIndent
	{
		public Indent(FileWriter fileWriter) : this(fileWriter, fileWriter.IndentLevel + 1)
		{
		}

		public Indent(FileWriter fileWriter, int indentLevel)
		{
			FileWriter = fileWriter;
			IndentLevel = FileWriter.IndentLevel;

			FileWriter.IndentLevel = indentLevel;
		}

		public int IndentLevel { get; set; }

		protected bool Disposed { get; set; }

		protected FileWriter FileWriter { get; set; }

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
			}

			Disposed = true;
		}
	}
}