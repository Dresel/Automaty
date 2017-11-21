namespace Automaty.Samples.HelloWorld.MSBuild
{
	using System.IO;

	public class HelloWorld
	{
		public void Execute()
		{
			File.WriteAllText("HelloWorld.generated.cs", "// Hello World!");
		}
	}
}