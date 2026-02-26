using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7);

        builder.HasData(
            new Category { Id = 1, Name = "Food & Dining", Color = "#FF6B6B" },
            new Category { Id = 2, Name = "Transportation", Color = "#4ECDC4" },
            new Category { Id = 3, Name = "Housing", Color = "#45B7D1" },
            new Category { Id = 4, Name = "Entertainment", Color = "#96CEB4" },
            new Category { Id = 5, Name = "Healthcare", Color = "#FFEAA7" },
            new Category { Id = 6, Name = "Shopping", Color = "#DDA0DD" },
            new Category { Id = 7, Name = "Education", Color = "#98D8C8" },
            new Category { Id = 8, Name = "Other", Color = "#B0BEC5" }
        );
    }
}
