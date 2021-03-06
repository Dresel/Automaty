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
			}

			Disposed = true;
		}
	}
}