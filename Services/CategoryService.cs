using MoneyFixClient.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pelo gerenciamento de categorias (contrato em docs/api/02-categories.md).
/// </summary>
public class CategoryService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    private static JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    /// <summary>
    /// Aceita array JSON na raiz ou objeto com várias chaves comuns (data, items, etc.).
    /// </summary>
    private static List<Category> DeserializeCategoriesList(string raw)
    {
        raw = raw.Trim().TrimStart('\uFEFF');
        if (string.IsNullOrWhiteSpace(raw))
            return new List<Category>();

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
                return DeserializeCategoryArray(root);

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var name in new[] { "data", "items", "categories", "results", "value", "payload" })
                {
                    if (root.TryGetProperty(name, out var inner) && inner.ValueKind == JsonValueKind.Array)
                        return DeserializeCategoryArray(inner);
                }

                // Objeto único (uma categoria)
                if (root.TryGetProperty("id", out _) || root.TryGetProperty("name", out _))
                {
                    var one = CategoryFromObject(root);
                    return one != null ? new List<Category> { one } : new List<Category>();
                }
            }

            Console.WriteLine($"CategoryService: Formato JSON inesperado para lista (primeiros 400 chars): {raw[..Math.Min(400, raw.Length)]}");
            return new List<Category>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Falha ao desserializar lista de categorias: {ex.Message}. Trecho: {raw[..Math.Min(300, raw.Length)]}");
            return new List<Category>();
        }
    }

    private static List<Category> DeserializeCategoryArray(JsonElement array)
    {
        var list = new List<Category>();
        foreach (var el in array.EnumerateArray())
        {
            var c = el.ValueKind == JsonValueKind.Object
                ? CategoryFromObject(el)
                : JsonSerializer.Deserialize<Category>(el.GetRawText(), JsonOptions);
            if (c != null && (!string.IsNullOrEmpty(c.Id) || !string.IsNullOrEmpty(c.Name)))
                list.Add(c);
        }

        return list;
    }

    private static Category? CategoryFromObject(JsonElement el)
    {
        try
        {
            var direct = JsonSerializer.Deserialize<Category>(el.GetRawText(), JsonOptions);
            if (direct != null && (!string.IsNullOrEmpty(direct.Id) || !string.IsNullOrEmpty(direct.Name)))
                return direct;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Desserialização direta falhou, tentando mapeamento manual: {ex.Message}");
        }

        try
        {
            var id = ReadStringProp(el, "id", "Id", "categoryId");
            var name = ReadStringProp(el, "name", "Name", "categoryName");
            var type = ReadCategoryType(el);
            var color = ReadStringProp(el, "color", "Color");
            DateTime? created = null;
            if (TryReadDateTime(el, "createdAtUtc", out var dt1))
                created = dt1;
            else if (TryReadDateTime(el, "CreatedAtUtc", out var dt2))
                created = dt2;
            else if (TryReadDateTime(el, "created_at_utc", out var dt3))
                created = dt3;

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(name))
                return null;

            return new Category
            {
                Id = id ?? string.Empty,
                Name = name ?? string.Empty,
                Type = type,
                Color = color ?? string.Empty,
                CreatedAtUtc = created
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Mapeamento manual de categoria falhou: {ex.Message}");
            return null;
        }
    }

    private static string? ReadStringProp(JsonElement el, params string[] names)
    {
        foreach (var n in names)
        {
            if (!el.TryGetProperty(n, out var p))
                continue;
            switch (p.ValueKind)
            {
                case JsonValueKind.String:
                    return p.GetString();
                case JsonValueKind.Number:
                    return p.GetRawText();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return p.GetBoolean().ToString();
                default:
                    return p.GetRawText();
            }
        }

        return null;
    }

    /// <summary>
    /// Lê <c>type</c> como número (1/2) ou texto legado (Entrada/Saida).
    /// </summary>
    private static int ReadCategoryType(JsonElement el)
    {
        foreach (var name in new[] { "type", "Type", "categoryType" })
        {
            if (!el.TryGetProperty(name, out var p))
                continue;

            if (p.ValueKind == JsonValueKind.Number && p.TryGetInt32(out var n))
                return n;

            if (p.ValueKind == JsonValueKind.String)
            {
                var s = p.GetString();
                if (int.TryParse(s, out var i) && i is CategoryType.Entrada or CategoryType.Saida)
                    return i;
                if (string.Equals(s, "Entrada", StringComparison.OrdinalIgnoreCase))
                    return CategoryType.Entrada;
                if (string.Equals(s, "Saida", StringComparison.OrdinalIgnoreCase))
                    return CategoryType.Saida;
            }
        }

        return 0;
    }

    private static bool TryReadDateTime(JsonElement el, string name, out DateTime value)
    {
        value = default;
        if (!el.TryGetProperty(name, out var p))
            return false;
        if (p.ValueKind == JsonValueKind.String && DateTime.TryParse(p.GetString(), out var dt))
        {
            value = dt;
            return true;
        }

        return false;
    }

    private static string FormatErrorBody(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Erro na operação.";
        try
        {
            var payload = JsonSerializer.Deserialize<CategoryApiErrorPayload>(raw, JsonOptions);
            if (!string.IsNullOrEmpty(payload?.Message))
                return payload.Message;
            if (payload?.Errors is { Count: > 0 })
                return string.Join(" ", payload.Errors);
        }
        catch
        {
            // ignora
        }

        return raw;
    }

    /// <summary>
    /// Cria uma nova categoria (POST /api/categories).
    /// </summary>
    public async Task<CreateCategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/categories", request);

            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Created && response.IsSuccessStatusCode)
            {
                var category = JsonSerializer.Deserialize<Category>(body, JsonOptions);
                if (category != null && !string.IsNullOrEmpty(category.Id))
                {
                    return new CreateCategoryResponse
                    {
                        Success = true,
                        Id = category.Id,
                        Category = category,
                        Message = "Categoria criada com sucesso!"
                    };
                }
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new CreateCategoryResponse
                {
                    Message = FormatErrorBody(body)
                };
            }

            Console.WriteLine($"CategoryService: Erro na criação - Status: {response.StatusCode}, Content: {body}");

            return new CreateCategoryResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao criar categoria: {ex.Message}");
            return new CreateCategoryResponse
            {
                Message = $"Erro ao criar categoria: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Lista categorias do usuário, opcionalmente filtradas por tipo (GET /api/categories?type=1|2).
    /// </summary>
    public async Task<List<Category>> GetCategoriesAsync(int? type = null)
    {
        try
        {
            var url = "/api/categories";
            if (type is int t && (t == CategoryType.Entrada || t == CategoryType.Saida))
                url += $"?type={t}";

            Console.WriteLine("CategoryService: Buscando categorias");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = DeserializeCategoriesList(content);

                Console.WriteLine($"CategoryService: {categories.Count} categorias encontradas");
                return categories;
            }

            Console.WriteLine($"CategoryService: Erro ao buscar categorias - Status: {response.StatusCode}");
            return new List<Category>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao buscar categorias: {ex.Message}");
            return new List<Category>();
        }
    }

    /// <summary>
    /// Obtém uma categoria por id (GET /api/categories/{id}).
    /// </summary>
    public async Task<Category?> GetCategoryByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        try
        {
            var response = await _httpClient.GetAsync($"/api/categories/{Uri.EscapeDataString(id)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Category>(content, JsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao obter categoria: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Atualiza uma categoria (PUT /api/categories/{id}).
    /// </summary>
    public async Task<CreateCategoryResponse> UpdateCategoryAsync(string categoryId, UpdateCategoryRequest request)
    {
        try
        {
            Console.WriteLine($"CategoryService: Atualizando categoria {categoryId}");

            var response = await _httpClient.PutAsJsonAsync($"/api/categories/{Uri.EscapeDataString(categoryId)}", request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var category = JsonSerializer.Deserialize<Category>(body, JsonOptions);
                if (category != null)
                {
                    return new CreateCategoryResponse
                    {
                        Success = true,
                        Id = category.Id,
                        Category = category,
                        Message = "Categoria atualizada com sucesso!"
                    };
                }

                return new CreateCategoryResponse
                {
                    Success = true,
                    Id = categoryId,
                    Message = "Categoria atualizada com sucesso!"
                };
            }

            Console.WriteLine($"CategoryService: Erro na atualização - Status: {response.StatusCode}, Content: {body}");

            return new CreateCategoryResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    HttpStatusCode.Unauthorized => "Você não está autorizado",
                    HttpStatusCode.Forbidden => "Acesso negado",
                    HttpStatusCode.NotFound => "Categoria não encontrada",
                    HttpStatusCode.Conflict => FormatErrorBody(body),
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao atualizar categoria: {ex.Message}");
            return new CreateCategoryResponse
            {
                Message = $"Erro ao atualizar categoria: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Remove uma categoria (DELETE /api/categories/{id}).
    /// </summary>
    public async Task<CreateCategoryResponse> DeleteCategoryAsync(string categoryId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/categories/{Uri.EscapeDataString(categoryId)}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new CreateCategoryResponse
                {
                    Success = true,
                    Message = "Categoria excluída com sucesso."
                };
            }

            return new CreateCategoryResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    HttpStatusCode.NotFound => "Categoria não encontrada",
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao excluir categoria: {ex.Message}");
            return new CreateCategoryResponse
            {
                Message = $"Erro ao excluir categoria: {ex.Message}"
            };
        }
    }

    private sealed class CategoryApiErrorPayload
    {
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
