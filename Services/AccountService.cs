using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MoneyFixClient.Models;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço de contas (contrato em docs/api/03-accounts.md).
/// </summary>
public class AccountService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    private static JsonSerializerOptions JsonOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
    };

    private static string FormatErrorBody(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "Erro na operação.";
        try
        {
            var payload = JsonSerializer.Deserialize<AccountApiErrorPayload>(raw, JsonOptions);
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
    /// Lista contas do usuário (GET /api/accounts).
    /// </summary>
    public async Task<List<Account>> GetAccountsAsync(bool? isActive = null)
    {
        try
        {
            var url = "/api/accounts";
            if (isActive.HasValue)
                url += $"?isActive={isActive.Value.ToString().ToLowerInvariant()}";

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<Account>>(content, JsonOptions);
                return list ?? new List<Account>();
            }

            Console.WriteLine($"AccountService: Erro ao listar contas - Status: {response.StatusCode}");
            return new List<Account>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AccountService: Exceção ao listar contas: {ex.Message}");
            return new List<Account>();
        }
    }

    /// <summary>
    /// Cria uma nova conta (POST /api/accounts — 201 Created).
    /// </summary>
    public async Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/accounts", request, JsonOptions);
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Created && response.IsSuccessStatusCode)
            {
                var account = JsonSerializer.Deserialize<Account>(body, JsonOptions);
                if (account != null && !string.IsNullOrEmpty(account.Id))
                {
                    return new AccountResponse
                    {
                        Success = true,
                        Id = account.Id,
                        Account = account,
                        Message = "Conta criada com sucesso!"
                    };
                }
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new AccountResponse
                {
                    Message = FormatErrorBody(body)
                };
            }

            Console.WriteLine($"AccountService: Erro na criação - Status: {response.StatusCode}, Content: {body}");

            return new AccountResponse
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
            Console.WriteLine($"AccountService: Exceção ao criar conta: {ex.Message}");
            return new AccountResponse
            {
                Message = $"Erro ao criar conta: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém uma conta por id (GET /api/accounts/{id}).
    /// </summary>
    public async Task<Account?> GetAccountByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        try
        {
            var response = await _httpClient.GetAsync($"/api/accounts/{Uri.EscapeDataString(id)}");

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Account>(content, JsonOptions);
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AccountService: Exceção ao obter conta: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Atualiza uma conta (PUT /api/accounts/{id}).
    /// </summary>
    public async Task<AccountResponse> UpdateAccountAsync(string accountId, UpdateAccountRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/accounts/{Uri.EscapeDataString(accountId)}", request, JsonOptions);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var account = JsonSerializer.Deserialize<Account>(body, JsonOptions);
                if (account != null)
                {
                    return new AccountResponse
                    {
                        Success = true,
                        Id = account.Id,
                        Account = account,
                        Message = "Conta atualizada com sucesso!"
                    };
                }

                return new AccountResponse
                {
                    Success = true,
                    Id = accountId,
                    Message = "Conta atualizada com sucesso!"
                };
            }

            Console.WriteLine($"AccountService: Erro na atualização - Status: {response.StatusCode}, Content: {body}");

            return new AccountResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    HttpStatusCode.Unauthorized => "Você não está autorizado",
                    HttpStatusCode.Forbidden => "Acesso negado",
                    HttpStatusCode.NotFound => "Conta não encontrada",
                    HttpStatusCode.Conflict => FormatErrorBody(body),
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AccountService: Exceção ao atualizar conta: {ex.Message}");
            return new AccountResponse
            {
                Message = $"Erro ao atualizar conta: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Remove uma conta (DELETE /api/accounts/{id} — 204 sem corpo).
    /// </summary>
    public async Task<AccountResponse> DeleteAccountAsync(string accountId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/accounts/{Uri.EscapeDataString(accountId)}");
            var body = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return new AccountResponse
                {
                    Success = true,
                    Id = accountId,
                    Message = "Conta excluída com sucesso."
                };
            }

            return new AccountResponse
            {
                Message = response.StatusCode switch
                {
                    HttpStatusCode.BadRequest => FormatErrorBody(body),
                    HttpStatusCode.NotFound => "Conta não encontrada",
                    _ => FormatErrorBody(body)
                }
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AccountService: Exceção ao excluir conta: {ex.Message}");
            return new AccountResponse
            {
                Message = $"Erro ao excluir conta: {ex.Message}"
            };
        }
    }

    private sealed class AccountApiErrorPayload
    {
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
