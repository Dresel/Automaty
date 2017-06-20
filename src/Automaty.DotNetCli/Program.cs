namespace Automaty.DotNetCli
{
	using System.Collections.Generic;
	using System.Linq;
	using Automaty.Common.Logging;
	using Automaty.Core;
	using Microsoft.Extensions.CommandLineUtils;

	public class Program
	{
		public static int Main(string[] args)
		{
			CommandLineApplication app = new CommandLineApplication(false);

			app.HelpOption("-? | -h | --help");

			app.Command("run", c =>
			{
				c.Description = "Runs one or many csharp files with Automaty.";

				CommandArgument sourceFilePaths = c.Argument("sources", "Path to source files", true);
				CommandOption projectFilePath = c.Option("-p |--project <project>",
					"Path to project file. Required if script relies on project and package references.",
					CommandOptionType.SingleValue);
				CommandOption isVerboseOutput = c.Option("-v |--verbose <verbose>", "Enables verbose output.",
					CommandOptionType.NoValue);

				c.OnExecute(() =>
				{
					if (!sourceFilePaths.Values.Any())
					{
						app.ShowHelp("run");
					}
					else
					{
						return RunAutomaty(sourceFilePaths.Values, projectFilePath.Value(), isVerboseOutput.HasValue()) ? 0 : -1;
					}

					return 0;
				});
			});

			app.OnExecute(() =>
			{
				app.ShowHelp();

				return 0;
			});

			app.Execute(args.Except(new[] { "--" }).ToArray());

			return 0;
		}

		private static bool RunAutomaty(IEnumerable<string> sourceFilePaths, string projectFilePath, bool isVerbose)
		{
			ILoggerFactory loggerFactory = new ConsoleLoggerFactory
			{
				IsVerbose = isVerbose
			};

			AutomatyRunner automaty = new AutomatyRunner(loggerFactory);
			return automaty.Execute(sourceFilePaths, projectFilePath);
		}
	}
}