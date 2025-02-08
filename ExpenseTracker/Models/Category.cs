namespace ExpenseTracker.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Budget { get; set; }
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
}
