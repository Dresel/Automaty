namespace Automaty.Common.Output
{
	using System;
	using System.Threading.Tasks;

	public interface IFileWriter : IDisposable
	{
		int IndentLevel { get; set; }

		IOutputSettings OutputSettings { get; set; }

		IIndent WithIndent();

		IIndent WithIndent(int indentLevel);

		IFileWriter WriteLine(string line);

		Task<IFileWriter> WriteLineAsync(string line);
	}
}