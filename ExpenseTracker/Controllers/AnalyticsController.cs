using AutoMapper;
using ExpenseTracker.Data;
using ExpenseTracker.DTOs;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalyticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AnalyticsController(ApplicationDbContext context,IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("most-expensive")]
    public async Task<ActionResult<ExpenseDto>> GetMostExpensive()
    {
        var expense = await _context.Expenses
            .Include(e => e.Category)
            .OrderByDescending(e => e.Amount)
            .FirstOrDefaultAsync();

        if (expense == null)
        {
            return NotFound("No expenses found.");
        }

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return expenseDto;
    }

    [HttpGet("least-expensive")]
    public async Task<ActionResult<ExpenseDto>> GetLeastExpensive()
    {
        var expense = await _context.Expenses
         .Include(e => e.Category)
         .OrderBy(e => e.Amount)
         .FirstOrDefaultAsync();

        if (expense == null)
        {
            return NotFound("No expenses found.");
        }

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return expenseDto;
    }

    [HttpGet("total-expenses")]
    public async Task<ActionResult<int>> GetTotalExpenses()
    {
        int count = await _context.Expenses.CountAsync();
        return Ok(count);
    }

    [HttpGet("average-daily")]
    public async Task<ActionResult<double>> GetAverageDailyExpenses(
    [FromQuery] DateTime fromDate,
    [FromQuery] DateTime toDate)
    {
        var dailyTotals = await _context.Expenses
            .Where(e => e.Date >= fromDate && e.Date <= toDate)
            .GroupBy(e => e.Date.Date)
            .Select(g => new
            {
                Day = g.Key,
                Total = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        if (!dailyTotals.Any())
        {
            return Ok(0.0);
        }

        double averageDaily = dailyTotals.Average(x => x.Total);
        return Ok(averageDaily);
    }

    [HttpGet("average-monthly")]
    public async Task<ActionResult<double>> GetAverageMonthlyExpenses(
    [FromQuery] DateTime fromDate,
    [FromQuery] DateTime toDate)
    {
        DateTime adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);

        var monthlyTotals = await _context.Expenses
            .Where(e => e.Date >= fromDate && e.Date <= adjustedToDate)
            .GroupBy(e => new { e.Date.Year, e.Date.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Total = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        if (!monthlyTotals.Any())
        {
            return Ok(0.0);
        }

        double averageMonthly = monthlyTotals.Average(x => x.Total);
        return Ok(averageMonthly);
    }

    [HttpGet("average-yearly")]
    public async Task<ActionResult<double>> GetAverageYearlyExpenses(
    [FromQuery] DateTime fromDate,
    [FromQuery] DateTime toDate)
    {
        DateTime adjustedToDate = toDate.Date.AddDays(1).AddTicks(-1);

        var yearlyTotals = await _context.Expenses
            .Where(e => e.Date >= fromDate && e.Date <= adjustedToDate)
            .GroupBy(e => e.Date.Year)
            .Select(g => new
            {
                Year = g.Key,
                Total = g.Sum(e => e.Amount)
            })
            .ToListAsync();

        if (!yearlyTotals.Any())
        {
            return Ok(0.0);
        }

        double averageYearly = yearlyTotals.Average(x => x.Total);
        return Ok(averageYearly);
    }

}
