namespace Automaty.Core.Logging
{
	using Automaty.Common.Logging;

	public class NullLoggerFactory : ILoggerFactory
	{
		public ILogger<T> CreateLogger<T>()
		{
			return new NullLogger<T>();
		}
	}
}