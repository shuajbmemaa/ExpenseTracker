using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<BudgetSetting> BudgetSettings => Set<BudgetSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Groceries", Budget = 500 },
            new Category { Id = 2, Name = "Entertainment", Budget = 200 },
            new Category { Id = 3, Name = "Utilities", Budget = 300 }
        );

        modelBuilder.Entity<BudgetSetting>().HasData(
            new BudgetSetting { Id = 1, OverallBudget = 1000 }
        );

        modelBuilder.Entity<Expense>().HasData(
            new Expense
            {
                Id = 1,
                Amount = 100,
                Date = new DateTime(2025, 1, 1),
                Description = "Weekly groceries",
                CategoryId = 1
            },
            new Expense
            {
                Id = 2,
                Amount = 50,
                Date = new DateTime(2025, 1, 3),
                Description = "Cinema ticket",
                CategoryId = 2
            }
        );
    }

}