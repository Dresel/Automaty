namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldProjectReferenceMultiTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.ProjectReferenceMulti/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.ProjectReferenceMulti.csproj";

		protected const string ClassLibraryProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.ProjectReferenceMulti.ClassLibrary/";

		protected const string ClassLibraryProjectFilePath = "Automaty.Samples.HelloWorld.ProjectReferenceMulti.ClassLibrary.csproj";

		[TestMethod]
		public void AutomatyHelloWorldProjectReferenceMultiGenerateFiles()
		{
			string sampleProjectDirectoryPath = HelloWorldProjectReferenceMultiTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = HelloWorldProjectReferenceMultiTest.ProjectFilePath;

			string classLibraryProjectDirectoryPath = HelloWorldProjectReferenceMultiTest.ClassLibraryProjectDirectoryPath.ToPlatformSpecificPath();
			string classLibraryProjectFilePath = HelloWorldProjectReferenceMultiTest.ClassLibraryProjectFilePath;

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