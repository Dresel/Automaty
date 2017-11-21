namespace Automaty.Samples.Test
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Loader;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class HelloWorldMSBuildTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.HelloWorld.MSBuild/";

		protected const string ProjectFilePath = "Automaty.Samples.HelloWorld.MSBuild.csproj";

		[TestMethod]
		public void AutomatyHelloWorldMSBuildGenerateFiles()
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

			Assert.AreEqual($"// Hello World!", File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath1)));
			Assert.AreEqual($"// Hello World!", File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath2)));

			Assembly assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(Path.Combine(sampleProjectDirectoryPath, "bin\\Debug\\netstandard1.6\\Automaty.Samples.HelloWorld.MSBuild.dll".ToPlatformSpecificPath())));
			Assert.IsTrue(assembly.GetTypes().Any(x => x.Name == "GeneratedClassPartOfCompilation"));
		}
	}
}