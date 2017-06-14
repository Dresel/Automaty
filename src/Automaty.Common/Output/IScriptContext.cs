namespace Automaty.Common.Output
{
	using System;

	public interface IScriptContext : IDisposable
	{
		IFileCollectionWriter Output { get; }

		string ProjectFilePath { get; }

		string ScriptFilePath { get; }
	}
}