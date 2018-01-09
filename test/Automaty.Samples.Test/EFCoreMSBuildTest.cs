namespace Automaty.Samples.Test
{
	using System.IO;
	using Automaty.Core;
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	[TestClass]
	public class EFCoreMSBuildTest
	{
		protected const string ProjectDirectoryPath = "./samples/Automaty.Samples.EFCore.MSBuild/";

		protected const string ProjectFilePath = "Automaty.Samples.EFCore.MSBuild.csproj";

		[TestMethod]
		public void EFCoreMSBuildGenerateFiles()
		{
			string sampleProjectDirectoryPath = EFCoreMSBuildTest.ProjectDirectoryPath.ToPlatformSpecificPath();
			string projectFilePath = EFCoreMSBuildTest.ProjectFilePath;

			string generatedFilePath = "Repository.Automaty.Blog.generated.cs";

			Helper.AssertSampleProjectDirectoryPathExists(sampleProjectDirectoryPath);
			Helper.AssertGeneratedFileDoesNotExist(sampleProjectDirectoryPath, generatedFilePath);

			Helper.DotNetRestore(sampleProjectDirectoryPath, projectFilePath);
			Helper.DotNetBuild(sampleProjectDirectoryPath, projectFilePath);

			Helper.AssertGeneratedFileExists(sampleProjectDirectoryPath, generatedFilePath);

			Assert.IsTrue(File.ReadAllText(Path.Combine(sampleProjectDirectoryPath, generatedFilePath))
				.Contains("public virtual Automaty.Samples.EFCore.MSBuild.Data.Blog Find(System.Int32 blogId)"));
		}
	}
}