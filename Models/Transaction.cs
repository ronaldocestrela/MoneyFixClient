using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para resposta de operações de transação
/// </summary>
public class TransactionResponse
{
    /// <summary>
    /// ID da transação
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Success => !string.IsNullOrEmpty(Id);

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Transação conforme contrato da API (GET/POST/PUT) e compatível com respostas legadas do maindashboard.
/// </summary>
public class Transaction
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [JsonPropertyName("accountName")]
    public string AccountName { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; } = string.Empty;

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    private string _type = string.Empty;

    /// <summary>
    /// Tipo da transação: "Entrada" ou "Saida" (API atual); também aceita 1/2 no JSON.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonConverter(typeof(TransactionTypeFlexibleJsonConverter))]
    public string Type
    {
        get => _type;
        set => _type = value ?? string.Empty;
    }

    /// <summary>
    /// Formato legado (maindashboard): 1 = despesa, 2 = receita.
    /// </summary>
    [JsonPropertyName("transactionsType")]
    public int? TransactionsTypeLegacy
    {
        set
        {
            if (value == 1)
                _type = "Saida";
            else if (value == 2)
                _type = "Entrada";
        }
    }

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("transactionAmount")]
    public decimal? TransactionAmountLegacy
    {
        set
        {
            if (value.HasValue)
                Amount = value.Value;
        }
    }

    [JsonPropertyName("occurredAt")]
    public DateTime OccurredAt { get; set; }

    [JsonPropertyName("transactionDate")]
    public DateTime? TransactionDateLegacy
    {
        set
        {
            if (value.HasValue)
                OccurredAt = value.Value;
        }
    }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("transactionDescription")]
    public string? TransactionDescriptionLegacy
    {
        set
        {
            if (value != null)
                Description = value;
        }
    }

    [JsonPropertyName("transactionCategoryId")]
    public string? TransactionCategoryIdLegacy
    {
        set
        {
            if (!string.IsNullOrEmpty(value))
                CategoryId = value;
        }
    }

    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime? CreatedAtLegacy
    {
        set
        {
            if (value.HasValue)
                CreatedAtUtc = value.Value;
        }
    }

    [JsonPropertyName("categoryColor")]
    public string CategoryColor { get; set; } = string.Empty;

    /// <summary>
    /// Indica se é uma despesa (Saída).
    /// </summary>
    public bool IsExpense => string.Equals(Type, "Saida", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Indica se é uma receita (Entrada).
    /// </summary>
    public bool IsIncome => string.Equals(Type, "Entrada", StringComparison.OrdinalIgnoreCase);
}
