namespace Automaty.Core.Logging
{
	public interface ILoggerFactory
	{
		ILogger<T> CreateLogger<T>();
	}
}