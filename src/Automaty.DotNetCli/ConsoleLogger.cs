namespace Automaty.DotNetCli
{
	using System;
	using Automaty.Core.Logging;

	public class ConsoleLogger<T> : ILogger<T>
	{
		public bool IsVerbose { get; set; }

		public void WriteDebug(string text)
		{
			if (!IsVerbose)
			{
				return;
			}

			ConsoleColor color = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine($"{typeof(T).Name}: {text}");

			Console.ForegroundColor = color;
		}

		public void WriteError(string text)
		{
			ConsoleColor color = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Red;
			Console.Error.WriteLine($"{typeof(T).Name}: {text}");

			Console.ForegroundColor = color;
		}

		public void WriteInfo(string text)
		{
			Console.WriteLine($"{typeof(T).Name}: {text}");
		}

		public void WriteWarning(string text)
		{
			ConsoleColor color = Console.ForegroundColor;

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"{typeof(T).Name}: {text}");

			Console.ForegroundColor = color;
		}
	}
}