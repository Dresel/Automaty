namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.csproj";

		[TestMethod]
		public void AutomatyHelloWorldGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldTest.ProjectFilePath;

			string sourceFilePath = "HelloWorld.cs";
			string generatedFilePath = "helloworld.txt";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.AutomatyRun(sampleProjectDirectoryPath, sourceFilePath);

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)), "Hello World!");
		}

		[TestMethod]
		public void AutomatyHelloWorldWithContextGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldTest.ProjectFilePath;

			string sourceFilePath = "HelloWorldWithContext.cs";
			string generatedFilePath = "helloworldwithcontext.txt";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.AutomatyRun(sampleProjectDirectoryPath, $"{sourceFilePath} --project {projectFilePath}");

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)),
				$"Hello World!{Environment.NewLine}");
		}
	}
}