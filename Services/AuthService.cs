using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MoneyFixClient.Models;
using MoneyFixClient.Providers;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pela autenticação do usuário
/// </summary>
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public AuthService(
        HttpClient httpClient, 
        ILocalStorageService localStorage,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Realiza o login do usuário
    /// </summary>
    /// <param name="loginRequest">Dados de login</param>
    /// <returns>Resultado do login</returns>
    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/login", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    Console.WriteLine($"Login bem-sucedido! Token recebido: {loginResponse.AccessToken[..20]}...");
                    
                    // Armazena o access token no localStorage
                    await _localStorage.SetItemAsync("authToken", loginResponse.AccessToken);
                    Console.WriteLine("Token armazenado no localStorage como 'authToken'");

                    // Log do ValidTo do JWT para depuração
                    try
                    {
                        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(loginResponse.AccessToken);
                        Console.WriteLine($"[DEBUG] JWT ValidTo: {jwt.ValidTo:O} (UTC). Agora: {DateTime.UtcNow:O} (UTC)");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[DEBUG] Falha ao ler JWT: {ex.Message}");
                    }
                    
                    // Armazena o refresh token também (opcional, para futuro uso)
                    await _localStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken);
                    Console.WriteLine("Refresh token armazenado no localStorage");
                    
                    // Armazena informações de expiração
                    await _localStorage.SetItemAsync("tokenExpiration", loginResponse.Expiration);
                    Console.WriteLine($"Expiração armazenada: {loginResponse.Expiration}");
                    
                    // Verifica se foi realmente salvo
                    var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                    Console.WriteLine($"Verificação: Token salvo existe? {!string.IsNullOrEmpty(savedToken)}");
                    
                    // Notifica o AuthenticationStateProvider sobre a mudança de estado
                    await ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated();
                    // Aguarda um pouco para garantir que o estado foi atualizado
                    await Task.Delay(100);
                    
                    loginResponse.Message = "Login realizado com sucesso!";
                    return loginResponse;
                }
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new LoginResponse 
            { 
                Message = !string.IsNullOrEmpty(errorContent) ? errorContent : "Credenciais inválidas" 
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse 
            { 
                Message = $"Erro ao fazer login: {ex.Message}" 
            };
        }
    }

    /// <summary>
    /// Realiza o logout do usuário
    /// </summary>
    public async Task LogoutAsync()
    {
        try
        {
            // Faz a chamada para a API de logout
            var response = await _httpClient.PostAsync("/api/account/logout", null);
            
            if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                Console.WriteLine("Logout realizado com sucesso na API");
            }
            else
            {
                Console.WriteLine($"Erro no logout da API: {response.StatusCode}");
                // Continua com o logout local mesmo se a API falhar
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao chamar API de logout: {ex.Message}");
            // Continua com o logout local mesmo se a API falhar
        }
        
        // Remove o token do localStorage
        await _localStorage.RemoveItemAsync("authToken");
        Console.WriteLine("Token removido do localStorage");
        
        // Remove o refresh token
        await _localStorage.RemoveItemAsync("refreshToken");
        Console.WriteLine("Refresh token removido do localStorage");
        
        // Remove informações de expiração
        await _localStorage.RemoveItemAsync("tokenExpiration");
        Console.WriteLine("Informações de expiração removidas do localStorage");
        
        // Notifica o AuthenticationStateProvider sobre o logout
        ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        Console.WriteLine("Estado de autenticação atualizado para deslogado");
    }

    /// <summary>
    /// Verifica se o usuário está autenticado
    /// </summary>
    /// <returns>True se estiver autenticado</returns>
    public async Task<bool> IsAuthenticatedAsync()
    {
        // Usa o AuthenticationStateProvider para checar autenticação real
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
    }

    /// <summary>
    /// Obtém o token atual
    /// </summary>
    /// <returns>Token JWT ou null se não existir</returns>
    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    /// <summary>
    /// Verifica se o token está expirado
    /// </summary>
    /// <param name="token">Token</param>
    /// <returns>True se expirado</returns>
    private async Task<bool> IsTokenExpired(string token)
    {
        try
        {
            // Como não é um JWT válido, verifica pela data salva no localStorage
            var tokenExpiration = await _localStorage.GetItemAsync<DateTime?>("tokenExpiration");
            return tokenExpiration.HasValue && tokenExpiration.Value < DateTime.UtcNow;
        }
        catch
        {
            return true; // Se não conseguir verificar, considera como expirado
        }
    }

    /// <summary>
    /// Obtém o refresh token atual
    /// </summary>
    /// <returns>Refresh token ou null se não existir</returns>
    public async Task<string?> GetRefreshTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("refreshToken");
    }

    /// <summary>
    /// Obtém informações completas do token
    /// </summary>
    /// <returns>Informações do token ou null</returns>
    public async Task<TokenInfo?> GetTokenInfoAsync()
    {
        var accessToken = await GetTokenAsync();
        var refreshToken = await GetRefreshTokenAsync();
        var expiration = await _localStorage.GetItemAsync<DateTime?>("tokenExpiration");

        if (string.IsNullOrEmpty(accessToken))
            return null;

        return new TokenInfo
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken ?? string.Empty,
            TokenType = "Bearer",
            ExpiresAt = expiration ?? DateTime.UtcNow.AddHours(-1), // Se não há expiração, considera expirado
            ExpiresIn = expiration.HasValue ? (int)(expiration.Value - DateTime.UtcNow).TotalSeconds : 0
        };
    }

    /// <summary>
    /// Realiza o registro de um novo usuário
    /// </summary>
    /// <param name="registerRequest">Dados de registro</param>
    /// <returns>Resultado do registro</returns>
    public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
    {
        try
        {
            // Cria o objeto para enviar à API (sem confirmPassword)
            var requestBody = new
            {
                email = registerRequest.Email,
                password = registerRequest.Password,
                userFirstName = registerRequest.UserFirstName,
                userLastName = registerRequest.UserLastName
            };

            var response = await _httpClient.PostAsJsonAsync("/api/account/register", requestBody);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                // A API pode retornar apenas o ID do usuário ou um objeto completo
                // Vamos tentar deserializar como string primeiro, depois como objeto
                string userId;
                try
                {
                    userId = JsonSerializer.Deserialize<string>(content) ?? string.Empty;
                }
                catch
                {
                    // Se falhar, tenta como objeto
                    var responseObj = JsonSerializer.Deserialize<Dictionary<string, object>>(content);
                    userId = responseObj?.GetValueOrDefault("id")?.ToString() ?? string.Empty;
                }

                return new RegisterResponse
                {
                    Success = true,
                    Message = "Usuário registrado com sucesso!",
                    UserId = userId
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new RegisterResponse
                {
                    Success = false,
                    Message = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.BadRequest => "Dados inválidos fornecidos",
                        System.Net.HttpStatusCode.Conflict => "Este email já está em uso",
                        System.Net.HttpStatusCode.UnprocessableEntity => "Dados não puderam ser processados",
                        _ => !string.IsNullOrEmpty(errorContent) ? errorContent : "Erro ao registrar usuário"
                    }
                };
            }
        }
        catch (HttpRequestException)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "Erro de conexão. Verifique sua internet e tente novamente."
            };
        }
        catch (Exception ex)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = $"Erro inesperado: {ex.Message}"
            };
        }
    }
}
