using ExpenseTracker.Data;
using ExpenseTracker.Models;
using ExpenseTracker.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Services;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly ApplicationDbContext _context;

    public ExpenseService(IExpenseRepository expenseRepository,ApplicationDbContext context)
    {
        _expenseRepository = expenseRepository;
        _context = context;
    }

    public async Task<List<Expense>> GetAllExpensesAsync()
    {
        return await _expenseRepository.GetAllExpensesAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        return await _expenseRepository.GetExpenseByIdAsync(id);
    }

    public async Task<int> GetTotalExpensesCountAsync()
    {
        return await _expenseRepository.GetTotalExpensesCountAsync();
    }

    public async Task<Expense?> GetMostExpensiveExpenseAsync()
    {
        return await _expenseRepository.GetMostExpensiveExpenseAsync();
    }

    public async Task<Expense?> GetLeastExpensiveExpenseAsync()
    {
        return await _expenseRepository.GetLeastExpensiveExpenseAsync();
    }

    /* public async Task<Expense> CreateExpenseAsync(Expense expense)
     {
         return await _expenseRepository.CreateExpenseAsync(expense);
     }

     public async Task UpdateExpenseAsync(Expense expense)
     {
         await _expenseRepository.UpdateExpenseAsync(expense);
     }*/
    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        // Validate category
        var category = await _context.Categories.FindAsync(expense.CategoryId);
        if (category == null)
        {
            throw new ArgumentException("Invalid category.");
        }

        // Validate overall budget
        var overallBudgetSetting = await _context.BudgetSettings.FindAsync(1);
        if (overallBudgetSetting == null)
        {
            throw new ArgumentException("Overall budget not configured.");
        }

        // Calculate current totals
        double currentCategoryTotal = await _context.Expenses
            .Where(e => e.CategoryId == expense.CategoryId)
        .SumAsync(e => e.Amount);

        double currentOverallTotal = await _context.Expenses
            .SumAsync(e => e.Amount);

        // Validate category budget
        if (currentCategoryTotal + expense.Amount > category.Budget)
        {
            throw new InvalidOperationException("Adding this expense exceeds the category budget.");
        }

        // Validate overall budget
        if (currentOverallTotal + expense.Amount > overallBudgetSetting.OverallBudget)
        {
            throw new InvalidOperationException("Adding this expense exceeds the overall budget.");
        }

        // If validation passes, create the expense
        return await _expenseRepository.CreateExpenseAsync(expense);
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        var category = await _context.Categories.FindAsync(expense.CategoryId);
        if (category == null)
        {
            throw new ArgumentException("Invalid category.");
        }

        var overallBudgetSetting = await _context.BudgetSettings.FindAsync(1);
        if (overallBudgetSetting == null)
        {
            throw new ArgumentException("Overall budget not configured.");
        }

        double categoryTotalExcludingCurrent = await _context.Expenses
            .Where(e => e.CategoryId == expense.CategoryId && e.Id != expense.Id)
        .SumAsync(e => e.Amount);

        double overallTotalExcludingCurrent = await _context.Expenses
            .Where(e => e.Id != expense.Id)
            .SumAsync(e => e.Amount);

        if (categoryTotalExcludingCurrent + expense.Amount > category.Budget)
        {
            throw new InvalidOperationException("Updating this expense exceeds the category budget.");
        }

        if (overallTotalExcludingCurrent + expense.Amount > overallBudgetSetting.OverallBudget)
        {
            throw new InvalidOperationException("Updating this expense exceeds the overall budget.");
        }

        await _expenseRepository.UpdateExpenseAsync(expense);
    }

    public async Task DeleteExpenseAsync(int id)
    {
        await _expenseRepository.DeleteExpenseAsync(id);
    }

    public async Task<double> GetAverageDailyExpensesAsync(DateTime fromDate, DateTime toDate)
    {
        var expenses = await _expenseRepository.GetExpensesByDateRangeAsync(fromDate, toDate);

        var dailyTotals = expenses
            .GroupBy(e => e.Date.Date)
            .Select(g => g.Sum(e => e.Amount))
            .ToList();

        return dailyTotals.Any() ? dailyTotals.Average() : 0.0;
    }

    public async Task<double> GetAverageMonthlyExpensesAsync(DateTime fromDate, DateTime toDate)
    {
        var expenses = await _expenseRepository.GetExpensesByDateRangeAsync(fromDate, toDate);

        var monthlyTotals = expenses
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => g.Sum(e => e.Amount))
            .ToList();

        return monthlyTotals.Any() ? monthlyTotals.Average() : 0.0;
    }

    public async Task<double> GetAverageYearlyExpensesAsync(DateTime fromDate, DateTime toDate)
    {
        var expenses = await _expenseRepository.GetExpensesByDateRangeAsync(fromDate, toDate);

        var yearlyTotals = expenses
            .GroupBy(e => e.Date.Year)
            .Select(g => g.Sum(e => e.Amount))
            .ToList();

        return yearlyTotals.Any() ? yearlyTotals.Average() : 0.0;
    }

}
