using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public sealed class MonthlyBudgetConfiguration : IEntityTypeConfiguration<MonthlyBudget>
{
    public void Configure(EntityTypeBuilder<MonthlyBudget> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.Year)
            .IsRequired();

        builder.Property(b => b.Month)
            .IsRequired();

        builder.HasIndex(b => new { b.UserId, b.CategoryId, b.Year, b.Month })
            .IsUnique();

        builder.HasOne(b => b.User)
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
