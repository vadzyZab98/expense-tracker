using ExpenseTracker.Core.Interfaces;
using ExpenseTracker.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpenseTracker.Persistence;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IIncomeCategoryRepository, IncomeCategoryRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IMonthlyBudgetRepository, MonthlyBudgetRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
