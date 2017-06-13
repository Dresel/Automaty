namespace Automaty.Core.Logging
{
	public interface ILogger<T>
	{
		void WriteDebug(string text);

		void WriteError(string text);

		void WriteInfo(string text);

		void WriteWarning(string text);
	}
}