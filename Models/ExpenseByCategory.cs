namespace MoneyFixClient.Models;

public class ExpenseByCategory
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Percentage { get; set; } = 0;
    public string Color { get; set; } = string.Empty;
}
