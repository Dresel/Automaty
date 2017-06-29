namespace Automaty.Samples.HelloWorld.MSBuild
{
	using Automaty.Common.Execution;
	using Automaty.Common.Output;

	public class HelloWorldByConvention : IAutomatyHost
	{
		public void Execute(IScriptContext context)
		{
			context.Output.Current.WriteLine("// Hello World!");
		}
	}
}