using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.Role).IsRequired().HasDefaultValue("User");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.Color).IsRequired();
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Date).IsRequired();
            entity.HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category).WithMany(c => c.Expenses).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Food & Dining",     Color = "#FF6B6B" },
            new Category { Id = 2, Name = "Transportation",    Color = "#4ECDC4" },
            new Category { Id = 3, Name = "Housing",           Color = "#45B7D1" },
            new Category { Id = 4, Name = "Entertainment",     Color = "#96CEB4" },
            new Category { Id = 5, Name = "Healthcare",        Color = "#FFEAA7" },
            new Category { Id = 6, Name = "Shopping",          Color = "#DDA0DD" },
            new Category { Id = 7, Name = "Education",         Color = "#98D8C8" },
            new Category { Id = 8, Name = "Other",             Color = "#B0BEC5" }
        );
    }
}
