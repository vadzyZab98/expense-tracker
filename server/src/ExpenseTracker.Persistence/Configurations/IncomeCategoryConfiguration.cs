using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public sealed class IncomeCategoryConfiguration : IEntityTypeConfiguration<IncomeCategory>
{
    public void Configure(EntityTypeBuilder<IncomeCategory> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Color)
            .IsRequired()
            .HasMaxLength(7);

        builder.HasData(
            new IncomeCategory { Id = 1, Name = "Salary", Color = "#4CAF50" },
            new IncomeCategory { Id = 2, Name = "Freelance", Color = "#2196F3" },
            new IncomeCategory { Id = 3, Name = "Investments", Color = "#FF9800" },
            new IncomeCategory { Id = 4, Name = "Gift", Color = "#E91E63" });
    }
}
