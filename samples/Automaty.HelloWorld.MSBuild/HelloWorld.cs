namespace Automaty.HelloWorld.MSBuild
{
	using Automaty.Common.Output;

	public class HelloWorld
	{
		public void Execute(IScriptContext context)
		{
			context.Output.Default.WriteLine("// Hello World!");
		}
	}
}