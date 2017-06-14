namespace Automaty.Common.Execution
{
	using Automaty.Common.Output;

	public interface IAutomatyHost
	{
		void Execute(IScriptContext scriptContext);
	}
}