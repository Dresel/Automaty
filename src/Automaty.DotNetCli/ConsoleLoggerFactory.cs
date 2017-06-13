namespace Automaty.DotNetCli
{
	using Automaty.Core.Logging;

	public class ConsoleLoggerFactory : ILoggerFactory
	{
		public bool IsVerbose { get; set; }

		public ILogger<T> CreateLogger<T>()
		{
			return new ConsoleLogger<T>
			{
				IsVerbose = IsVerbose
			};
		}
	}
}