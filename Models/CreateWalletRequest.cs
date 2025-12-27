namespace MoneyFixClient.Models;

public class CreateWalletRequest
{
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}
