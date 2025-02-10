﻿namespace ExpenseTracker.Models;

public class Expense
{
    public int Id { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}

