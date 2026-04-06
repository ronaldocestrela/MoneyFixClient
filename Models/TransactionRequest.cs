using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Corpo JSON para criação/atualização de transação (POST/PUT /api/transactions).
/// O tipo é derivado da categoria na API; não enviar campo type.
/// </summary>
public class TransactionRequest
{
    [Required(ErrorMessage = "Conta é obrigatória")]
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Categoria é obrigatória")]
    [JsonPropertyName("categoryId")]
    public string CategoryId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 999999999.99, ErrorMessage = "Valor deve ser maior que zero")]
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Data é obrigatória")]
    [JsonPropertyName("occurredAt")]
    public DateTime OccurredAt { get; set; } = DateTime.Now;

    [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
