using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Valores numéricos do tipo de categoria na API (enum): <see cref="Entrada"/> = 1, <see cref="Saida"/> = 2.
/// </summary>
public static class CategoryType
{
    public const int Entrada = 1;
    public const int Saida = 2;
}

/// <summary>
/// Resultado de operações de categoria (criação, atualização, exclusão) e reutilizado por carteiras com o mesmo padrão de resposta.
/// </summary>
public class CreateCategoryResponse
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida (deve ser definido explicitamente nos serviços).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// ID retornado (criação de carteira ou ID da categoria quando aplicável).
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Categoria completa quando a API retorna o recurso (POST/PUT categorias).
    /// </summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Mensagem de retorno ou erro.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Modelo para representação de uma categoria retornada pela API.
/// </summary>
public class Category
{
    /// <summary>
    /// ID único da categoria
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Nome da categoria
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo numérico: 1 = Entrada, 2 = Saída (conforme enum da API).
    /// </summary>
    [JsonPropertyName("type")]
    public int Type { get; set; }

    /// <summary>
    /// Cor em hexadecimal (#RRGGBB)
    /// </summary>
    [JsonPropertyName("color")]
    public string Color { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação em UTC
    /// </summary>
    [JsonPropertyName("createdAtUtc")]
    public DateTime? CreatedAtUtc { get; set; }
}

/// <summary>
/// Corpo JSON para atualização de categoria (sem id no body; o id vai na URL).
/// </summary>
public class UpdateCategoryRequest
{
    /// <summary>
    /// Nome da categoria
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo: 1 = Entrada, 2 = Saída
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Cor da categoria
    /// </summary>
    public string Color { get; set; } = string.Empty;
}
