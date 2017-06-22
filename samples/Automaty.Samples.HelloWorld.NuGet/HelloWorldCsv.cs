namespace Automaty.Samples.HelloWorld.NuGet
{
	using System.IO;
	using Automaty.Common.Output;
	using CsvHelper;

	public class HelloWorldCsv
	{
		public void Execute(IScriptContext context)
		{
			using (CsvWriter csvWriter = new CsvWriter(File.CreateText("helloworld.csv")))
			{
				csvWriter.WriteField("Hello");
				csvWriter.WriteField("World");
				csvWriter.WriteField("!");
				csvWriter.NextRecord();
			}
		}
	}
}