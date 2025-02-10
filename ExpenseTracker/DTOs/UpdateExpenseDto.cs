namespace ExpenseTracker.DTOs;

public class UpdateExpenseDto
{
    public string? Description { get; set; }
    public double? Amount { get; set; }
    public DateTime? Date { get; set; }
    public int? CategoryId { get; set; }
}
