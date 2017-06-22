namespace Automaty.Samples.EFCore.MSBuild.Data
{
	using System.Collections.Generic;

	public class Tag
	{
		public List<PostTag> PostTags { get; set; }

		public string TagId { get; set; }
	}
}