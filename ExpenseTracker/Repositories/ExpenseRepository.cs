using ExpenseTracker.Data;
using ExpenseTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ApplicationDbContext _context;

    public ExpenseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Expense>> GetAllExpensesAsync()
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .ToListAsync();
    }

    public async Task<Expense?> GetExpenseByIdAsync(int id)
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
        return expense;
    }

    public async Task UpdateExpenseAsync(Expense expense)
    {
        _context.Entry(expense).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpenseAsync(int id)
    {
        var expense = await GetExpenseByIdAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        DateTime adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);
        return await _context.Expenses
            .Include(e => e.Category)
            .Where(e => e.Date >= fromDate && e.Date <= adjustedToDate)
            .ToListAsync();
    }

    public async Task<Expense?> GetMostExpensiveExpenseAsync()
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .OrderByDescending(e => e.Amount)
            .FirstOrDefaultAsync();
    }

    public async Task<Expense?> GetLeastExpensiveExpenseAsync()
    {
        return await _context.Expenses
            .Include(e => e.Category)
            .OrderBy(e => e.Amount)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalExpensesCountAsync()
    {
        return await _context.Expenses.CountAsync();
    }
}
