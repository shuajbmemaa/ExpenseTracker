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
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly IMapper _mapper;

    public ExpensesController(IExpenseService expenseService, IMapper mapper)
    {
        _expenseService = expenseService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpenses()
    {
        var expenses = await _expenseService.GetAllExpensesAsync();
        var expenseDtos = _mapper.Map<List<ExpenseDto>>(expenses);
        return Ok(expenseDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseById(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        var expenseDto = _mapper.Map<ExpenseDto>(expense);
        return Ok(expenseDto);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto createExpenseDto)
    {
        try
        {
            var expense = _mapper.Map<Expense>(createExpenseDto);
            var createdExpense = await _expenseService.CreateExpenseAsync(expense);
            var expenseDto = _mapper.Map<ExpenseDto>(createdExpense);
            return CreatedAtAction(nameof(GetExpenseById), new { id = expenseDto.Id }, expenseDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExpense(int id, UpdateExpenseDto updateExpenseDto)
    {
        try
        {
            var expense = await _expenseService.GetExpenseByIdAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _mapper.Map(updateExpenseDto, expense);
            await _expenseService.UpdateExpenseAsync(expense);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpense(int id)
    {
        var expense = await _expenseService.GetExpenseByIdAsync(id);
        if (expense == null)
        {
            return NotFound();
        }
        await _expenseService.DeleteExpenseAsync(id);
        return NoContent();
    }

}