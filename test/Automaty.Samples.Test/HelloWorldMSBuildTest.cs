namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldMSBuildTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.MSBuild/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.MSBuild.csproj";

		[TestMethod]
		public void AutomatyHelloWorldGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldMSBuildTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldMSBuildTest.ProjectFilePath;

			string generatedFilePath1 = "HelloWorld.generated.cs";
			string generatedFilePath2 = "HelloWorldByConvention.Automaty.generated.cs";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath1);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath2);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.DotNetBuild(sampleProjectDirectoryPath, projectFilePath);

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath1);
			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath2);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath1)), $"// Hello World!{Environment.NewLine}");
			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath2)), $"// Hello World!{Environment.NewLine}");

		}
	}
}