namespace Automaty.Samples.HelloWorld.MSBuild
{
	using System.IO;

	public class HelloWorldPartOfCompilation
	{
		public void Execute()
		{
			File.WriteAllText("HelloWorldPartOfCompilation.generated.cs", "public class GeneratedClassPartOfCompilation { }");
		}
	}
}