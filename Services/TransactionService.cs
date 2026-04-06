using MoneyFixClient.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pelo gerenciamento de transações (contrato em docs/api/04-transactions.md).
/// </summary>
public class TransactionService
{
    private readonly HttpClient _httpClient;

    private static JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    /// <summary>
    /// Array na raiz ou objeto com data/items/transactions; item a item para não perder a lista inteira por um campo inesperado.
    /// </summary>
    private static List<Transaction> DeserializeTransactionsList(string raw)
    {
        raw = raw.Trim().TrimStart('\uFEFF');
        if (string.IsNullOrWhiteSpace(raw))
            return new List<Transaction>();

        try
        {
            using var doc = JsonDocument.Parse(raw);
            var root = doc.RootElement;

            if (root.ValueKind == JsonValueKind.Array)
                return DeserializeTransactionArray(root);

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var name in new[] { "data", "items", "transactions", "results", "value", "payload" })
                {
                    if (root.TryGetProperty(name, out var inner) && inner.ValueKind == JsonValueKind.Array)
                        return DeserializeTransactionArray(inner);
                }
            }

            Console.WriteLine($"TransactionService: Formato JSON inesperado para lista (trecho): {raw[..Math.Min(200, raw.Length)]}");
            return new List<Transaction>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Falha ao interpretar lista de transações: {ex.Message}");
            return new List<Transaction>();
        }
    }

    private static List<Transaction> DeserializeTransactionArray(JsonElement array)
    {
        var list = new List<Transaction>();
        foreach (var el in array.EnumerateArray())
        {
            try
            {
                var tx = JsonSerializer.Deserialize<Transaction>(el.GetRawText(), JsonOptions);
                if (tx != null)
                    list.Add(tx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TransactionService: Item de transação ignorado na lista: {ex.Message}");
            }
        }

        return list;
    }

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private static string FormatErrorBody(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Erro na operação.";
        try
        {
            var payload = JsonSerializer.Deserialize<TransactionApiErrorPayload>(raw, JsonOptions);
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
    /// Cria uma nova transação (POST /api/transactions — resposta 201 com corpo completo).
    /// </summary>
    public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest request)
    {
        try
        {
            Console.WriteLine($"TransactionService: Criando transação — conta {request.AccountId}, categoria {request.CategoryId}");

            var response = await _httpClient.PostAsJsonAsync("/api/transactions", request, JsonOptions);

            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Created && response.IsSuccessStatusCode)
            {
                var tx = JsonSerializer.Deserialize<Transaction>(body, JsonOptions);
                if (tx != null && !string.IsNullOrEmpty(tx.Id))
                {
                    return new TransactionResponse
                    {
                        Id = tx.Id,
                        Message = "Transação criada com sucesso!"
                    };
                }
            }

            Console.WriteLine($"TransactionService: Erro na criação - Status: {response.StatusCode}, Content: {body}");

            return new TransactionResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    HttpStatusCode.Unauthorized => "Você não está autenticado",
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao criar transação: {ex.Message}");
            return new TransactionResponse
            {
                Message = $"Erro ao criar transação: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Atualiza uma transação (PUT /api/transactions/{id} — resposta 200 com corpo completo).
    /// </summary>
    public async Task<TransactionResponse> UpdateTransactionAsync(string id, TransactionRequest request)
    {
        try
        {
            Console.WriteLine($"TransactionService: Atualizando transação {id}");

            var response = await _httpClient.PutAsJsonAsync($"/api/transactions/{Uri.EscapeDataString(id)}", request, JsonOptions);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var tx = JsonSerializer.Deserialize<Transaction>(body, JsonOptions);
                if (tx != null && !string.IsNullOrEmpty(tx.Id))
                {
                    return new TransactionResponse
                    {
                        Id = tx.Id,
                        Message = "Transação atualizada com sucesso!"
                    };
                }

                return new TransactionResponse
                {
                    Id = id,
                    Message = "Transação atualizada com sucesso!"
                };
            }

            var errorMessage = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => FormatErrorBody(body),
                HttpStatusCode.Unauthorized => "Você não está autenticado",
                HttpStatusCode.Forbidden => "Você não tem permissão para editar esta transação",
                HttpStatusCode.NotFound => "Transação não encontrada",
                _ => FormatErrorBody(body)
            };

            Console.WriteLine($"TransactionService: Erro na atualização - Status: {response.StatusCode}, Message: {errorMessage}");

            return new TransactionResponse
            {
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao atualizar transação: {ex.Message}");
            return new TransactionResponse
            {
                Message = $"Erro ao atualizar transação: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Exclui uma transação (DELETE /api/transactions/{id} — 204 sem corpo).
    /// </summary>
    public async Task<TransactionResponse> DeleteTransactionAsync(string id)
    {
        try
        {
            Console.WriteLine($"TransactionService: Excluindo transação {id}");

            var response = await _httpClient.DeleteAsync($"/api/transactions/{Uri.EscapeDataString(id)}");

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new TransactionResponse
                {
                    Id = id,
                    Message = "Transação excluída com sucesso!"
                };
            }

            var body = await response.Content.ReadAsStringAsync();
            var errorMessage = response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => "Você não está autenticado",
                HttpStatusCode.Forbidden => "Você não tem permissão para excluir esta transação",
                HttpStatusCode.NotFound => "Transação não encontrada",
                _ => FormatErrorBody(body)
            };

            Console.WriteLine($"TransactionService: Erro na exclusão - Status: {response.StatusCode}, Message: {errorMessage}");

            return new TransactionResponse
            {
                Message = errorMessage
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao excluir transação: {ex.Message}");
            return new TransactionResponse
            {
                Message = $"Erro ao excluir transação: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Lista transações com filtros opcionais (GET /api/transactions).
    /// </summary>
    public async Task<List<Transaction>> GetTransactionsAsync(TransactionFilter? filter = null)
    {
        try
        {
            var url = "/api/transactions";
            var query = BuildQuery(filter);
            if (!string.IsNullOrEmpty(query))
                url += "?" + query;

            Console.WriteLine($"TransactionService: Buscando transações {url}");

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transactions = DeserializeTransactionsList(content);
                Console.WriteLine($"TransactionService: {transactions.Count} transações encontradas");
                return transactions;
            }

            Console.WriteLine($"TransactionService: Erro ao buscar transações - Status: {response.StatusCode}");
            return new List<Transaction>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao buscar transações: {ex.Message}");
            return new List<Transaction>();
        }
    }

    private static string BuildQuery(TransactionFilter? filter)
    {
        if (filter == null)
            return string.Empty;

        var parts = new List<string>();
        if (filter.From.HasValue)
            parts.Add($"from={Uri.EscapeDataString(filter.From.Value.ToUniversalTime().ToString("o"))}");
        if (filter.To.HasValue)
            parts.Add($"to={Uri.EscapeDataString(filter.To.Value.ToUniversalTime().ToString("o"))}");
        if (!string.IsNullOrWhiteSpace(filter.Type))
            parts.Add($"type={Uri.EscapeDataString(filter.Type)}");
        if (!string.IsNullOrWhiteSpace(filter.CategoryId))
            parts.Add($"categoryId={Uri.EscapeDataString(filter.CategoryId)}");
        if (!string.IsNullOrWhiteSpace(filter.AccountId))
            parts.Add($"accountId={Uri.EscapeDataString(filter.AccountId)}");

        return string.Join("&", parts);
    }

    /// <summary>
    /// Lista com paginação por header (legado — pageNumber/pageSize se o servidor suportar).
    /// </summary>
    public async Task<PaginatedResult<List<Transaction>>> GetTransactionsAsync(int pageNumber = 1, int pageSize = 1000)
    {
        try
        {
            Console.WriteLine($"TransactionService: Buscando transações - página {pageNumber}, tamanho {pageSize}");

            var response = await _httpClient.GetAsync($"/api/transactions?pageNumber={pageNumber}&pageSize={pageSize}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transactions = DeserializeTransactionsList(content);

                Pagination? pagination = null;
                if (response.Headers.TryGetValues("Pagination", out var values))
                {
                    var header = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(header))
                    {
                        try
                        {
                            pagination = JsonSerializer.Deserialize<Pagination>(header, JsonOptions);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"TransactionService: Falha ao desserializar header Pagination: {ex.Message}");
                        }
                    }
                }

                Console.WriteLine($"TransactionService: {transactions.Count} transações encontradas (pagina: {pagination?.CurrentPage})");
                return new PaginatedResult<List<Transaction>>
                {
                    Items = transactions,
                    Pagination = pagination
                };
            }

            Console.WriteLine($"TransactionService: Erro ao buscar transações - Status: {response.StatusCode}");
            return new PaginatedResult<List<Transaction>> { Items = new List<Transaction>() };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao buscar transações: {ex.Message}");
            return new PaginatedResult<List<Transaction>> { Items = new List<Transaction>() };
        }
    }

    /// <summary>
    /// Obtém uma transação por id (GET /api/transactions/{id}).
    /// </summary>
    public async Task<Transaction?> GetTransactionByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        try
        {
            var response = await _httpClient.GetAsync($"/api/transactions/{Uri.EscapeDataString(id)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Transaction>(content, JsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao obter transação: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Últimas transações do dashboard.
    /// </summary>
    public async Task<List<Transaction>> GetLastTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/maindashboard/last-transactions?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return DeserializeTransactionsList(content);
            }

            return new List<Transaction>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao buscar últimas transações: {ex.Message}");
            return new List<Transaction>();
        }
    }

    /// <summary>
    /// Totais do período (dashboard).
    /// </summary>
    public async Task<Profit> GetTotalTransactionsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/maindashboard/profit?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var totalTransactions = JsonSerializer.Deserialize<Profit>(content, JsonOptions);

                return totalTransactions ?? new Profit();
            }

            return new Profit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao buscar total de transações: {ex.Message}");
            return new Profit();
        }
    }

    /// <summary>
    /// Despesas por categoria (dashboard).
    /// </summary>
    public async Task<List<ExpenseByCategory>> GetExpensesByCategoryAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        try
        {
            var url = "/api/maindashboard/spending-percentage";
            if (startDate.HasValue || endDate.HasValue)
            {
                var query = new List<string>();
                if (startDate.HasValue)
                    query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
                if (endDate.HasValue)
                    query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
                url += "?" + string.Join("&", query);
            }

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var expensesByCategory = JsonSerializer.Deserialize<List<ExpenseByCategory>>(content, JsonOptions);

                return expensesByCategory ?? new List<ExpenseByCategory>();
            }

            return new List<ExpenseByCategory>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TransactionService: Exceção ao buscar despesas por categoria: {ex.Message}");
            return new List<ExpenseByCategory>();
        }
    }

    private sealed class TransactionApiErrorPayload
    {
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
