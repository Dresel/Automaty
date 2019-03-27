namespace Automaty.Samples.EFCore3Net3.MSBuild.Data
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Tag
	{
		public List<PostTag> PostTags { get; set; }

		[Required]
		public string TagId { get; set; }
	}
}