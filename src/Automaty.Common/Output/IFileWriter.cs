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

		IFileWriter WriteLine();

		IFileWriter WriteLine(string value);

		Task<IFileWriter> WriteLineAsync();

		Task<IFileWriter> WriteLineAsync(string value);

		IIndent WriteScope();

		IIndent WriteScope(string header);
	}
}