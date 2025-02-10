using AutoMapper;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using ExpenseTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IMapper _mapper;

    public AnalyticsController(IExpenseService expenseService, IMapper mapper)
    {
        _expenseService = expenseService;
        _mapper = mapper;
    }

    [HttpGet("average-daily")]
    public async Task<ActionResult<double>> GetAverageDailyExpenses(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        double averageDaily = await _expenseService.GetAverageDailyExpensesAsync(fromDate, toDate);
        return Ok(averageDaily);
    }

    [HttpGet("average-monthly")]
    public async Task<ActionResult<double>> GetAverageMonthlyExpenses(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        double averageMonthly = await _expenseService.GetAverageMonthlyExpensesAsync(fromDate, toDate);
        return Ok(averageMonthly);
    }

    [HttpGet("average-yearly")]
    public async Task<ActionResult<double>> GetAverageYearlyExpenses(
        [FromQuery] DateTime fromDate,
        [FromQuery] DateTime toDate)
    {
        double averageYearly = await _expenseService.GetAverageYearlyExpensesAsync(fromDate, toDate);
        return Ok(averageYearly);
    }

    [HttpGet("total-expenses")]
    public async Task<ActionResult<int>> GetTotalExpenses()
    {
        int totalCount = await _expenseService.GetTotalExpensesCountAsync();
        return Ok(totalCount);
    }

    [HttpGet("most-expensive")]
    public async Task<ActionResult<ExpenseDto>> GetMostExpensiveExpense()
    {
        Expense? expense = await _expenseService.GetMostExpensiveExpenseAsync();
        if (expense == null)
        {
            return NotFound("No expenses found.");
        }

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Ok(expenseDto);
    }

    [HttpGet("least-expensive")]
    public async Task<ActionResult<ExpenseDto>> GetLeastExpensiveExpense()
    {
        Expense? expense = await _expenseService.GetLeastExpensiveExpenseAsync();
        if (expense == null)
        {
            return NotFound("No expenses found.");
        }

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Ok(expenseDto);
    }
}
