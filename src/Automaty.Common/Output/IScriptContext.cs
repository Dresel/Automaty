namespace Automaty.Common.Output
{
	using System;
	using Automaty.Common.Logging;

	public interface IScriptContext : IDisposable
	{
		IFileCollectionWriter Output { get; }

		string ProjectFilePath { get; }

		string ScriptFilePath { get; }

		ILogger<IScriptContext> Logger { get; set; }
	}
}