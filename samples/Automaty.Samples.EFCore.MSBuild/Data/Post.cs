namespace Automaty.Samples.EFCore.MSBuild.Data
{
	using System.Collections.Generic;

	public class Post
	{
		public Blog Blog { get; set; }

		public int BlogId { get; set; }

		public string Content { get; set; }

		public int PostId { get; set; }

		public List<PostTag> PostTags { get; set; }

		public string Title { get; set; }
	}
}