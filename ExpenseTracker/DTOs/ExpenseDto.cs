namespace ExpenseTracker.DTOs;

public class ExpenseDto
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
}
