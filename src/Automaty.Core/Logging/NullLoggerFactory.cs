namespace Automaty.Core.Logging
{
	public class NullLoggerFactory : ILoggerFactory
	{
		public ILogger<T> CreateLogger<T>()
		{
			return new NullLogger<T>();
		}
	}
}