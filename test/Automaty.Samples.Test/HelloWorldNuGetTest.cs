namespace Automaty.Samples.Test
{
	using System;
	using System.Globalization;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldNuGetTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.NuGet/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.NuGet.csproj";

		[TestMethod]
		public void AutomatyHelloWorldNuGetCsvGenerateFiles()
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

			Assert.AreEqual(
				$"Hello{CultureInfo.CurrentCulture.TextInfo.ListSeparator}World{CultureInfo.CurrentCulture.TextInfo.ListSeparator}!{Environment.NewLine}",
				File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)));
		}

		[TestMethod]
		public void AutomatyHelloWorldNuGetJsonGenerateFiles()
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

			Assert.AreEqual($"\"Hello World!\"{Environment.NewLine}",
				File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)));
		}
	}
}