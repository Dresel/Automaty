namespace Automaty.Samples.Test
{
	using System;
	using System.Globalization;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldNugetMultiTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.NugetMulti/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.NugetMulti.csproj";

		[TestMethod]
		public void AutomatyHelloWorldNugetMultiCsvGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNugetMultiTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNugetMultiTest.ProjectFilePath;

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
		public void AutomatyHelloWorldNugetMultiJsonGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNugetMultiTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNugetMultiTest.ProjectFilePath;

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