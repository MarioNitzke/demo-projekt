using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PhysioBook.Data.Entities;

public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public ApplicationUser Author { get; set; } = null!;
}

public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.AuthorId)
            .IsRequired();

        builder.HasOne(a => a.Author)
            .WithMany()
            .HasForeignKey(a => a.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
