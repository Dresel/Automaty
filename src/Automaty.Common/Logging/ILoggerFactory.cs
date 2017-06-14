namespace Automaty.Common.Logging
{
	public interface ILoggerFactory
	{
		ILogger<T> CreateLogger<T>();
	}
}