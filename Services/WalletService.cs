using MoneyFixClient.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço para gerenciar carteiras do usuário
/// </summary> <summary>
public class WalletService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// Cria uma nova carteira
    /// </summary>
    /// <param name="request">Dados da carteira a ser criada</param>
    /// <returns>Em caso de sucesso, retorna o ID da carteira criada, em caso de erro, retorna uma mensagem de erro</returns>
    public async Task<CreateCategoryResponse> CreateWalletAsync(CreateWalletRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/wallets/add-wallet", request);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"WalletService: Resposta da API: {content}");
                
                // A API retorna apenas o ID como string
                var walletId = JsonSerializer.Deserialize<string>(content);
                
                if (!string.IsNullOrEmpty(walletId))
                {
                    return new CreateCategoryResponse
                    {
                        Success = true,
                        Id = walletId,
                        Message = "Carteira criada com sucesso!"
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"WalletService: Erro na criação - Status: {response.StatusCode}, Content: {errorContent}");
            
            return new CreateCategoryResponse
            {
                Message = !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao criar carteira"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WalletService: Exceção ao criar carteira: {ex.Message}");
            return new CreateCategoryResponse
            {
                Message = $"Erro ao criar carteira: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Obtém todas as carteiras do usuário
    /// </summary>
    /// <returns>Lista de carteiras</returns>
    public async Task<List<Wallet>> GetWalletsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/wallets/get-wallets");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"WalletService: Resposta da API: {content}");
                
                var wallets = JsonSerializer.Deserialize<List<Wallet>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return wallets ?? new List<Wallet>();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"WalletService: Erro ao obter carteiras - Status: {response.StatusCode}, Content: {errorContent}");
            return new List<Wallet>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WalletService: Exceção ao obter carteiras: {ex.Message}");
            return new List<Wallet>();
        }
    }
}
