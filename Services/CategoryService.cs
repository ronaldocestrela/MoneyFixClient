using MoneyFixClient.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pelo gerenciamento de categorias
/// </summary>
public class CategoryService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// Cria uma nova categoria
    /// </summary>
    /// <param name="request">Dados da categoria a ser criada</param>
    /// <returns>Resultado da criação</returns>
    public async Task<CreateCategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/categories/add-category", request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CategoryService: Resposta da API: {content}");
                
                // A API retorna apenas o ID como string
                var categoryId = JsonSerializer.Deserialize<string>(content);
                
                if (!string.IsNullOrEmpty(categoryId))
                {
                    return new CreateCategoryResponse
                    {
                        Id = categoryId,
                        Message = "Categoria criada com sucesso!"
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CategoryService: Erro na criação - Status: {response.StatusCode}, Content: {errorContent}");
            
            return new CreateCategoryResponse
            {
                Message = !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao criar categoria"
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
    /// Lista todas as categorias do usuário
    /// </summary>
    /// <returns>Lista de categorias</returns>
    public async Task<List<Category>> GetCategoriesAsync()
    {
        try
        {
            Console.WriteLine("CategoryService: Buscando categorias");
            
            var response = await _httpClient.GetAsync("/api/categories");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<Category>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                Console.WriteLine($"CategoryService: {categories?.Count ?? 0} categorias encontradas");
                return categories ?? new List<Category>();
            }

            Console.WriteLine($"CategoryService: Erro ao buscar categorias - Status: {response.StatusCode}");
            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao buscar categorias: {ex.Message}");
            return [];
        }
    }

    /// <summary>
    /// Atualiza uma categoria existente
    /// </summary>
    /// <param name="categoryId">ID da categoria a ser atualizada</param>
    /// <param name="request">Dados atualizados da categoria</param>
    /// <returns>Resultado da atualização</returns>
    public async Task<CreateCategoryResponse> UpdateCategoryAsync(string categoryId, CreateCategoryRequest request)
    {
        try
        {
            Console.WriteLine($"CategoryService: Atualizando categoria {categoryId}");
            
            var response = await _httpClient.PutAsJsonAsync($"/api/categories/{categoryId}", request);
            
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("CategoryService: Categoria atualizada com sucesso");
                
                return new CreateCategoryResponse
                {
                    Id = categoryId,
                    Message = "Categoria atualizada com sucesso!"
                };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CategoryService: Erro na atualização - Status: {response.StatusCode}, Content: {errorContent}");
            
            return new CreateCategoryResponse
            {
                Message = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => "Dados inválidos fornecidos",
                    System.Net.HttpStatusCode.Unauthorized => "Você não está autorizado",
                    System.Net.HttpStatusCode.Forbidden => "Você não tem permissão para editar esta categoria",
                    System.Net.HttpStatusCode.NotFound => "Categoria não encontrada",
                    _ => !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao atualizar categoria"
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
}
