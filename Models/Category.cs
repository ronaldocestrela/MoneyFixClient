namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para resposta de criação de categoria
/// </summary>
public class CreateCategoryResponse
{
    /// <summary>
    /// ID da categoria criada
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Indica se a criação foi bem-sucedida
    /// </summary>
    public bool Success => !string.IsNullOrEmpty(Id);
    
    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Modelo para representação de uma categoria
/// </summary>
public class Category
{
    /// <summary>
    /// ID único da categoria
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// ID do usuário proprietário
    /// </summary>
    public string UserId { get; set; } = string.Empty;
}
