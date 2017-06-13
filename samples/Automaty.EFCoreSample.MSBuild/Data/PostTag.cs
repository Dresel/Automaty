namespace Automaty.EFCoreSample.MSBuild.Data
{
	public class PostTag
	{
		public Post Post { get; set; }

		public int PostId { get; set; }

		public Tag Tag { get; set; }

		public string TagId { get; set; }
	}
}