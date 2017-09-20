namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldProjectReferenceTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.ProjectReference/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.ProjectReference.csproj";

		protected const string ClassLibraryProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.ProjectReference.ClassLibrary/";

		protected const string ClassLibraryProjectFilePath = "Automaty.Samples.HelloWorld.ProjectReference.ClassLibrary.csproj";

		[TestMethod]
		public void AutomatyHelloWorldProjectReferenceGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldProjectReferenceTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldProjectReferenceTest.ProjectFilePath;

			string classLibraryProjectDirectoryPath = HelloWorldProjectReferenceTest.ClassLibraryProjectDirectoryPath.ToPlatformSpecificPath();
			string classLibraryProjectFilePath = HelloWorldProjectReferenceTest.ClassLibraryProjectFilePath;

			string sourceFilePath = "HelloWorld.cs";
			string generatedFilePath = "helloworld.txt";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(classLibraryProjectDirectoryPath, classLibraryProjectFilePath);
			Helper.DotNetBuild(classLibraryProjectDirectoryPath, classLibraryProjectFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.AutomatyRun(sampleProjectDirectoryPath, $"{sourceFilePath} --project {projectFilePath}");

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.AreEqual(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath)), $"Automaty.Samples.HelloWorld.ProjectReference.ClassLibrary.Class{Environment.NewLine}");
		}
	}
}