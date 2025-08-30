using System.ComponentModel.DataAnnotations;

namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para solicitação de criação de categoria
/// </summary>
public class CreateCategoryRequest
{
    /// <summary>
    /// Nome da categoria
    /// </summary>
    [Required(ErrorMessage = "Nome da categoria é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
    public string Name { get; set; } = string.Empty;
}
