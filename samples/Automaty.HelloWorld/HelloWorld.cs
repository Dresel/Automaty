namespace Automaty.HelloWorld
{
	using System.IO;

	public class HelloWorld
	{
		public void Execute()
		{
			File.WriteAllText("helloworld.txt", "Hello World!");
		}
	}
}