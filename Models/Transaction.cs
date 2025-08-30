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
/// Modelo para representação completa de uma transação
/// </summary>
public class Transaction
{
    /// <summary>
    /// ID único da transação
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Descrição da transação
    /// </summary>
    public string TransactionDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Valor da transação
    /// </summary>
    public decimal TransactionAmount { get; set; }
    
    /// <summary>
    /// Data da transação
    /// </summary>
    public DateTime TransactionDate { get; set; }
    
    /// <summary>
    /// Tipo da transação (1 = Despesa, 2 = Receita)
    /// </summary>
    public int TransactionsType { get; set; }
    
    /// <summary>
    /// ID da categoria da transação
    /// </summary>
    public string TransactionCategoryId { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome da categoria (para exibição)
    /// </summary>
    public string CategoryName { get; set; } = string.Empty;
    
    /// <summary>
    /// ID do usuário proprietário
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Data da última atualização
    /// </summary>
    public DateTime UpdatedAt { get; set; }
    
    /// <summary>
    /// Tipo da transação como enum
    /// </summary>
    public TransactionType Type => (TransactionType)TransactionsType;
    
    /// <summary>
    /// Indica se é uma despesa
    /// </summary>
    public bool IsExpense => TransactionsType == 1;
    
    /// <summary>
    /// Indica se é uma receita
    /// </summary>
    public bool IsIncome => TransactionsType == 2;
}
