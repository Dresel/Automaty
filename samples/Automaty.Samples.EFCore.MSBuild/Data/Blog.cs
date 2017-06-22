namespace Automaty.Samples.EFCore.MSBuild.Data
{
	using System.Collections.Generic;

	public class Blog
	{
		public int BlogId { get; set; }

		public List<Post> Posts { get; set; }

		public string Url { get; set; }
	}
}