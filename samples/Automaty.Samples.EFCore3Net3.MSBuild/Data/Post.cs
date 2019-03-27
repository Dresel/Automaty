namespace Automaty.Samples.EFCore3Net3.MSBuild.Data
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Post
	{
		public Blog Blog { get; set; }

		public int BlogId { get; set; }

		[Required]
		public string Content { get; set; }

		public int PostId { get; set; }

		public List<PostTag> PostTags { get; set; }

		[Required]
		public string Title { get; set; }
	}
}