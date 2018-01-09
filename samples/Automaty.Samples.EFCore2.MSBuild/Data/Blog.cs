namespace Automaty.Samples.EFCore2.MSBuild.Data
{
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Blog
	{
		public int BlogId { get; set; }

		public List<Post> Posts { get; set; }

		[Required]
		public string Url { get; set; }
	}
}