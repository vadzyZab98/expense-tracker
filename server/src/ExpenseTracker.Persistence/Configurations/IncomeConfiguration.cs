using ExpenseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Persistence.Configurations;

public sealed class IncomeConfiguration : IEntityTypeConfiguration<Income>
{
    public void Configure(EntityTypeBuilder<Income> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(i => i.Date)
            .IsRequired();

        builder.HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.IncomeCategory)
            .WithMany(c => c.Incomes)
            .HasForeignKey(i => i.IncomeCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
