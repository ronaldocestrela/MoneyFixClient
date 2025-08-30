using System.ComponentModel.DataAnnotations;

namespace MoneyFixClient.Models;

/// <summary>
/// Enum para tipos de transação
/// </summary>
public enum TransactionType
{
    Expense = 1,    // Despesa
    Income = 2      // Receita
}

/// <summary>
/// Modelo para solicitação de criação/atualização de transação
/// </summary>
public class TransactionRequest
{
    /// <summary>
    /// Descrição da transação
    /// </summary>
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 200 caracteres")]
    public string TransactionDescription { get; set; } = string.Empty;

    /// <summary>
    /// Valor da transação
    /// </summary>
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, 999999.99, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal TransactionAmount { get; set; }

    /// <summary>
    /// Data da transação
    /// </summary>
    [Required(ErrorMessage = "Data é obrigatória")]
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    /// <summary>
    /// Tipo da transação (1 = Despesa, 2 = Receita)
    /// </summary>
    [Required(ErrorMessage = "Tipo de transação é obrigatório")]
    [Range(1, 2, ErrorMessage = "Tipo deve ser 1 (Despesa) ou 2 (Receita)")]
    public int TransactionsType { get; set; } = 1;

    /// <summary>
    /// ID da categoria da transação
    /// </summary>
    [Required(ErrorMessage = "Categoria é obrigatória")]
    public string TransactionCategoryId { get; set; } = string.Empty;
}
