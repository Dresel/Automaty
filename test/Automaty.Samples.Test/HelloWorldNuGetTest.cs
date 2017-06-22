namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldNuGetTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.NuGet/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.NuGet.csproj";

		[TestMethod]
		public void AutomatyHelloWorldJsonGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNuGetTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNuGetTest.ProjectFilePath;

			string sourceFilePath = "HelloWorldJson.cs";
			string generatedFilePath = "helloworld.json";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.AutomatyRun(sampleProjectDirectoryPath, $"{sourceFilePath} --project {projectFilePath}");

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)),
				$"\"Hello World!\"{Environment.NewLine}");
		}

		[TestMethod]
		public void AutomatyHelloWorldCsvGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNuGetTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNuGetTest.ProjectFilePath;

			string sourceFilePath = "HelloWorldCsv.cs";
			string generatedFilePath = "helloworld.csv";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.AutomatyRun(sampleProjectDirectoryPath, $"{sourceFilePath} --project {projectFilePath}");

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)),
				$"Hello;World;!{Environment.NewLine}");
		}
	}
}