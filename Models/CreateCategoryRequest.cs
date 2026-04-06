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
    [StringLength(120, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 120 caracteres")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo numérico: 1 = Entrada (receita), 2 = Saída (despesa)
    /// </summary>
    [Range(1, 2, ErrorMessage = "Tipo deve ser 1 (Entrada) ou 2 (Saída)")]
    public int Type { get; set; } = CategoryType.Saida;

    /// <summary>
    /// Cor em formato hexadecimal (#RRGGBB)
    /// </summary>
    [Required(ErrorMessage = "Cor é obrigatória")]
    public string Color { get; set; } = string.Empty;
}
