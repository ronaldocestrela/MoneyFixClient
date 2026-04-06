using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Tipo de conta na API (valor numérico no JSON).
/// </summary>
public enum AccountType
{
    ContaCorrente = 1,
    Poupanca = 2,
    Carteira = 3
}

/// <summary>
/// Corpo JSON para criação de conta (POST /api/accounts).
/// </summary>
public class CreateAccountRequest
{
    /// <summary>Nome da conta (máx. 120 caracteres).</summary>
    [Required(ErrorMessage = "Nome da conta é obrigatório")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 120 caracteres")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>1 = Conta corrente, 2 = Poupança, 3 = Carteira.</summary>
    [Required(ErrorMessage = "Tipo de conta é obrigatório")]
    [Range(1, 3, ErrorMessage = "Tipo de conta inválido")]
    [JsonPropertyName("type")]
    public AccountType Type { get; set; } = AccountType.ContaCorrente;

    /// <summary>Saldo inicial (≥ 0). Não use [Required] em decimal — 0 é válido e o validador trataria 0 como inválido.</summary>
    [Range(typeof(decimal), "0", "9999999999999999", ErrorMessage = "Saldo inicial não pode ser negativo")]
    [JsonPropertyName("initialBalance")]
    public decimal InitialBalance { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Corpo JSON para atualização de conta (PUT /api/accounts/{id}). O tipo não é alterável pelo cliente.
/// </summary>
public class UpdateAccountRequest
{
    [Required(ErrorMessage = "Nome da conta é obrigatório")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Nome deve ter entre 1 e 120 caracteres")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("initialBalance")]
    public decimal InitialBalance { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Resultado de operações de conta (criação, atualização, exclusão).
/// </summary>
public class AccountResponse
{
    public bool Success { get; set; }

    public string Id { get; set; } = string.Empty;

    public Account? Account { get; set; }

    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Conta bancária/carteira conforme <c>GET /api/accounts</c> (docs/api/03-accounts.md).
/// </summary>
public class Account
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public AccountType Type { get; set; }

    [JsonPropertyName("initialBalance")]
    public decimal InitialBalance { get; set; }

    [JsonPropertyName("currentBalance")]
    public decimal CurrentBalance { get; set; }

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    [JsonPropertyName("createdAtUtc")]
    public DateTime? CreatedAtUtc { get; set; }
}
