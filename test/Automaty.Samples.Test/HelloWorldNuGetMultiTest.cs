namespace Automaty.Samples.Test
{
	using System;
	using System.Globalization;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldNuGetMultiTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.NuGetMulti/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.NuGetMulti.csproj";

		[TestMethod]
		public void AutomatyHelloWorldNuGetMultiCsvGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNuGetMultiTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNuGetMultiTest.ProjectFilePath;

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
		public void AutomatyHelloWorldNuGetMultiJsonGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldNuGetMultiTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldNuGetMultiTest.ProjectFilePath;

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