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
public class ExpensesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ExpensesController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
    {
        var expense = await _context.Expenses
            .Include(s => s.Category)
            .ToListAsync();

        var expenseDto = _mapper.Map<List<ExpenseDto>>(expense);

        return Ok(expenseDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id)
    {
        var expense = await _context.Expenses
            .Include(s => s.Category)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null)
        {
            return NotFound();
        }

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Ok(expenseDto);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense == null)
        {
            return NotFound();
        }

        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto createExpenseDto)
    {
        var expense = _mapper.Map<Expense>(createExpenseDto);

        var category = await _context.Categories.FindAsync(expense.CategoryId);
        if (category == null)
        {
            return BadRequest("Invalid category.");
        }

        var overallBudgetSetting = await _context.BudgetSettings.FindAsync(1);
        if (overallBudgetSetting == null)
        {
            return BadRequest("Overall budget not configured.");
        }

        double currentCategoryTotal = await _context.Expenses
            .Where(e => e.CategoryId == expense.CategoryId)
            .SumAsync(e => e.Amount);

        double currentOverallTotal = await _context.Expenses
            .SumAsync(e => e.Amount);

        if (currentCategoryTotal + expense.Amount > category.Budget)
        {
            return BadRequest("Adding this expense exceeds the category budget.");
        }

        if (currentOverallTotal + expense.Amount > overallBudgetSetting.OverallBudget)
        {
            return BadRequest("Adding this expense exceeds the overall budget.");
        }

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, expenseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, UpdateExpenseDto updateExpenseDto)
    {
        var expense = await _context.Expenses.FirstOrDefaultAsync(e => e.Id == id);
        if (expense == null)
        {
            return NotFound();
        }

        _mapper.Map(updateExpenseDto, expense);

        var category = await _context.Categories.FindAsync(expense.CategoryId);
        if (category == null)
        {
            return BadRequest("Invalid category.");
        }

        var overallBudgetSetting = await _context.BudgetSettings.FindAsync(1);
        if (overallBudgetSetting == null)
        {
            return BadRequest("Overall budget not configured.");
        }

        double categoryTotalExcludingCurrent = await _context.Expenses
            .Where(e => e.CategoryId == expense.CategoryId && e.Id != id)
            .SumAsync(e => e.Amount);

        double overallTotalExcludingCurrent = await _context.Expenses
            .Where(e => e.Id != id)
            .SumAsync(e => e.Amount);

        if (categoryTotalExcludingCurrent + expense.Amount > category.Budget)
        {
            return BadRequest("Updating this expense exceeds the category budget.");
        }

        if (overallTotalExcludingCurrent + expense.Amount > overallBudgetSetting.OverallBudget)
        {
            return BadRequest("Updating this expense exceeds the overall budget.");
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

}
