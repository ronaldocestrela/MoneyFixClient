namespace MoneyFixClient.Models;

public class Wallet
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int TransactionsInMonth { get; set; }
    public string Color { get; set; } = string.Empty;
}
