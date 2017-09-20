namespace Automaty.Samples.HelloWorld.ProjectReference
{
	using Automaty.Common.Execution;
	using Automaty.Common.Output;
	using Automaty.Samples.HelloWorld.ProjectReference.ClassLibrary;

	public class HelloWorld : IAutomatyHost
	{
		public void Execute(IScriptContext context)
		{
			context.Output["helloworld.txt"].WriteLine(typeof(Class).FullName);
		}
	}
}