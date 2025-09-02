namespace MoneyFixClient.Models;

public class Profit
{
    public decimal TotalIncome { get; set; } = 0.00M;
    public decimal TotalExpense { get; set; } = 0.00M;
    public decimal NetProfit { get; set; } = 0.00M;
    public int TransactionCount { get; set; } = 0;
}
