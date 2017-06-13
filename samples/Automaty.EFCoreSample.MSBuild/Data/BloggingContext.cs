namespace Automaty.EFCoreSample.MSBuild.Data
{
	using Microsoft.EntityFrameworkCore;

	public class BloggingContext : DbContext
	{
		public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<PostTag>()
				.HasKey(t => new { t.PostId, t.TagId });

			modelBuilder.Entity<PostTag>()
				.HasOne(pt => pt.Post)
				.WithMany(p => p.PostTags)
				.HasForeignKey(pt => pt.PostId);

			modelBuilder.Entity<PostTag>()
				.HasOne(pt => pt.Tag)
				.WithMany(t => t.PostTags)
				.HasForeignKey(pt => pt.TagId);
		}

		public DbSet<Blog> Blogs { get; set; }

		public DbSet<Post> Posts { get; set; }

		public DbSet<PostTag> PostTags { get; set; }

		public DbSet<Tag> Tags { get; set; }
	}
}