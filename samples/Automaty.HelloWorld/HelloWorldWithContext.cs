namespace Automaty.HelloWorld
{
	using Automaty.Common.Execution;
	using Automaty.Common.Output;

	public class HelloWorldWithContext : IAutomatyHost
	{
		public void Execute(IScriptContext context)
		{
			context.Output["helloworldwithcontext.txt"].WriteLine("Hello World!");
		}
	}
}