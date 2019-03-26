namespace Automaty.Samples.Test
{
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class EFCore3Net3MSBuildTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.EFCore3Net3.MSBuild/";

		protected const string ProjectFilePath = "Automaty.Samples.EFCore3Net3.MSBuild.csproj";

		[TestMethod]
		public void EFCore3Net3MSBuildGenerateFiles()
		{
			string sampleProjectDirectoryPath = EFCore3Net3MSBuildTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = EFCore3Net3MSBuildTest.ProjectFilePath;

			string generatedFilePath = "Repository.Automaty.Blog.generated.cs";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.DotNetBuild(sampleProjectDirectoryPath, projectFilePath);

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.IsTrue(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath))
				.Contains("public virtual Automaty.Samples.EFCore3Net3.MSBuild.Data.Blog Find(System.Int32 blogId)"));
		}
	}
}