using ExpenseTracker.DTOs;
using ExpenseTracker.Models;

namespace ExpenseTracker.Services;

public interface IExpenseService
{
    Task<List<Expense>> GetAllExpensesAsync();
    Task<Expense?> GetExpenseByIdAsync(int id);
    Task<Expense> CreateExpenseAsync(Expense expense);
    Task UpdateExpenseAsync(Expense expense);
    Task DeleteExpenseAsync(int id);
    Task<double> GetAverageDailyExpensesAsync(DateTime fromDate, DateTime toDate);
    Task<double> GetAverageMonthlyExpensesAsync(DateTime fromDate, DateTime toDate);
    Task<double> GetAverageYearlyExpensesAsync(DateTime fromDate, DateTime toDate);
    Task<int> GetTotalExpensesCountAsync();
    Task<Expense?> GetMostExpensiveExpenseAsync();
    Task<Expense?> GetLeastExpensiveExpenseAsync();
}
