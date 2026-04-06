namespace MoneyFixClient.Models;

/// <summary>
/// Filtros opcionais para GET /api/transactions (query string).
/// </summary>
public class TransactionFilter
{
    /// <summary>Início do período (opcional).</summary>
    public DateTime? From { get; set; }

    /// <summary>Fim do período (opcional).</summary>
    public DateTime? To { get; set; }

    /// <summary>"Entrada" ou "Saida" (opcional).</summary>
    public string? Type { get; set; }

    public string? CategoryId { get; set; }

    public string? AccountId { get; set; }
}
