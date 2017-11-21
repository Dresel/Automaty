namespace Automaty.Samples.HelloWorld.MSBuild
{
	using System.IO;

	public class HelloWorldByConvention
	{
		public void Execute()
		{
			File.WriteAllText("HelloWorldByConvention.Automaty.generated.cs", "// Hello World!");
		}
	}
}