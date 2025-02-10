using ExpenseTracker.Models;

namespace ExpenseTracker.Repositories;

public interface IExpenseRepository
{
    Task<List<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(int id);
    Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<Expense?> GetMostExpensiveExpenseAsync();
    Task<Expense?> GetLeastExpensiveExpenseAsync();
    Task<int> GetTotalExpensesCountAsync();
}
