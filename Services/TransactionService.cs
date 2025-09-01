using MoneyFixClient.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pelo gerenciamento de transações
/// </summary>
public class TransactionService
{
    private readonly HttpClient _httpClient;

    public TransactionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Cria uma nova transação
    /// </summary>
    /// <param name="request">Dados da transação a ser criada</param>
    /// <returns>Resultado da criação</returns>
    public async Task<TransactionResponse> CreateTransactionAsync(TransactionRequest request)
    {
        try
        {
            Console.WriteLine($"TransactionService: Criando transação '{request.TransactionDescription}'");
            Console.WriteLine($"TransactionService: Valor: {request.TransactionAmount:C}, Tipo: {request.TransactionsType}");

            var response = await _httpClient.PostAsJsonAsync("/api/transactions", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"TransactionService: Resposta da API: {content}");

                // A API retorna apenas o ID como string
                var transactionId = JsonSerializer.Deserialize<string>(content);

                if (!string.IsNullOrEmpty(transactionId))
                {
                    Console.WriteLine($"TransactionService: Transação criada com sucesso! ID: {transactionId}");
                    return new TransactionResponse
                    {
                        Id = transactionId,
                        Message = "Transação criada com sucesso!"
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"TransactionService: Erro na criação - Status: {response.StatusCode}, Content: {errorContent}");

            return new TransactionResponse
            {
                Message = !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao criar transação"
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
    /// Atualiza uma transação existente
    /// </summary>
    /// <param name="id">ID da transação</param>
    /// <param name="request">Dados atualizados da transação</param>
    /// <returns>Resultado da atualização</returns>
    public async Task<TransactionResponse> UpdateTransactionAsync(string id, TransactionRequest request)
    {
        try
        {
            Console.WriteLine($"TransactionService: Atualizando transação {id}");
            Console.WriteLine($"TransactionService: Nova descrição: '{request.TransactionDescription}'");

            var response = await _httpClient.PutAsJsonAsync($"/api/transactions/{id}", request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"TransactionService: Resposta da API: {content}");

                // A API retorna o ID da transação atualizada
                var transactionId = JsonSerializer.Deserialize<string>(content);

                if (!string.IsNullOrEmpty(transactionId))
                {
                    Console.WriteLine($"TransactionService: Transação atualizada com sucesso! ID: {transactionId}");
                    return new TransactionResponse
                    {
                        Id = transactionId,
                        Message = "Transação atualizada com sucesso!"
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => "Dados inválidos fornecidos",
                System.Net.HttpStatusCode.Unauthorized => "Você não está autenticado",
                System.Net.HttpStatusCode.Forbidden => "Você não tem permissão para editar esta transação",
                System.Net.HttpStatusCode.NotFound => "Transação não encontrada",
                _ => !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao atualizar transação"
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
    /// Exclui uma transação
    /// </summary>
    /// <param name="id">ID da transação</param>
    /// <returns>Resultado da exclusão</returns>
    public async Task<TransactionResponse> DeleteTransactionAsync(string id)
    {
        try
        {
            Console.WriteLine($"TransactionService: Excluindo transação {id}");

            var response = await _httpClient.DeleteAsync($"/api/transactions/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"TransactionService: Resposta da API: {content}");

                // A API retorna o ID da transação excluída
                var transactionId = JsonSerializer.Deserialize<string>(content);

                if (!string.IsNullOrEmpty(transactionId))
                {
                    Console.WriteLine($"TransactionService: Transação excluída com sucesso! ID: {transactionId}");
                    return new TransactionResponse
                    {
                        Id = transactionId,
                        Message = "Transação excluída com sucesso!"
                    };
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            var errorMessage = response.StatusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Você não está autenticado",
                System.Net.HttpStatusCode.Forbidden => "Você não tem permissão para excluir esta transação",
                System.Net.HttpStatusCode.NotFound => "Transação não encontrada",
                _ => !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao excluir transação"
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
    /// Lista todas as transações do usuário
    /// </summary>
    /// <returns>Lista de transações</returns>
    public async Task<List<Transaction>> GetTransactionsAsync()
    {
        try
        {
            Console.WriteLine("TransactionService: Buscando transações");

            var response = await _httpClient.GetAsync("/api/transactions");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transactions = JsonSerializer.Deserialize<List<Transaction>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                Console.WriteLine($"TransactionService: {transactions?.Count ?? 0} transações encontradas");
                return transactions ?? new List<Transaction>();
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
    
    /// <summary>
    /// Lista as ultimas 5 tranzações do usuário
    /// </summary>
    /// <returns>Lista de transações</returns>
    public async Task<List<Transaction>> GetLastTransactionsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/maindashboard/last-transactions");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var transactions = JsonSerializer.Deserialize<List<Transaction>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return transactions ?? [];
            }

            return [];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CategoryService: Exceção ao buscar transações: {ex.Message}");
            return [];
        }
    }
}
