using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MoneyFixClient.Models;
using MoneyFixClient.Providers;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço responsável pela autenticação do usuário (rotas sob o prefixo /api/identity da API).
/// Caminhos sem "/" inicial para combinar com <see cref="ApiBaseUrlResolver"/> (ex.: base .../api/ + identity/register).
/// </summary>
public class AuthService(
    HttpClient httpClient,
    ILocalStorageService localStorage,
    AuthenticationStateProvider authenticationStateProvider,
    TokenRefreshCoordinator tokenRefreshCoordinator)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider = authenticationStateProvider;
    private readonly TokenRefreshCoordinator _tokenRefreshCoordinator = tokenRefreshCoordinator;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Realiza o login do usuário (POST .../api/identity/login).
    /// </summary>
    public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("identity/login", loginRequest, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, JsonOptions);

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    await StoreTokensAndNotifyAsync(loginResponse, cancellationToken);
                    loginResponse.Message = "Login realizado com sucesso!";
                    return loginResponse;
                }
            }

            return new LoginResponse
            {
                Message = await ReadErrorMessageAsync(response, cancellationToken)
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
    /// Registra usuário e persiste tokens (POST .../api/identity/register).
    /// </summary>
    public async Task<LoginResponse> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("identity/register", registerRequest, cancellationToken);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, JsonOptions);
                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.AccessToken))
                {
                    await StoreTokensAndNotifyAsync(loginResponse, cancellationToken);
                    loginResponse.Message = "Conta criada com sucesso!";
                    return loginResponse;
                }

                return new LoginResponse
                {
                    Message = "Resposta de registro inválida."
                };
            }

            return new LoginResponse
            {
                Message = await ReadErrorMessageFromContentAsync(response.StatusCode, content, cancellationToken)
            };
        }
        catch (HttpRequestException)
        {
            return new LoginResponse
            {
                Message = "Erro de conexão. Verifique sua internet e tente novamente."
            };
        }
        catch (Exception ex)
        {
            return new LoginResponse
            {
                Message = $"Erro inesperado: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// Renova o access token (POST /identity/refresh).
    /// </summary>
    public Task<bool> RefreshTokenAsync(CancellationToken cancellationToken = default)
        => _tokenRefreshCoordinator.TryRefreshAsync(cancellationToken);

    /// <summary>
    /// Solicita token de redefinição de senha (POST .../api/identity/forgot-password).
    /// </summary>
    public async Task<(bool Success, ForgotPasswordResponse? Data, string Error)> ForgotPasswordAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var request = new ForgotPasswordRequest { Email = email };
        var response = await _httpClient.PostAsJsonAsync("identity/forgot-password", request, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            return (false, null, await ReadErrorMessageFromContentAsync(response.StatusCode, content, cancellationToken));

        var data = JsonSerializer.Deserialize<ForgotPasswordResponse>(content, JsonOptions);
        return (true, data, string.Empty);
    }

    /// <summary>
    /// Redefine a senha (POST .../api/identity/reset-password). Resposta 204 sem corpo em caso de sucesso.
    /// </summary>
    public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync("identity/reset-password", request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NoContent)
            return (true, string.Empty);

        var message = await ReadErrorMessageAsync(response, cancellationToken);
        return (false, message);
    }

    /// <summary>
    /// Perfil do usuário autenticado (GET .../api/identity/me).
    /// </summary>
    public async Task<UserProfile?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("identity/me", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return null;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<UserProfile>(content, JsonOptions);
    }

    /// <summary>
    /// Realiza o logout (POST .../api/identity/logout).
    /// </summary>
    public async Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync("identity/logout", null, cancellationToken);

            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NoContent)
            {
                // ok
            }
        }
        catch (Exception)
        {
            // Continua com o logout local mesmo se a API falhar
        }

        await _localStorage.RemoveItemAsync("authToken", cancellationToken);
        await _localStorage.RemoveItemAsync("refreshToken", cancellationToken);
        await _localStorage.RemoveItemAsync("tokenExpiration", cancellationToken);

        ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
    }

    /// <summary>
    /// Verifica se o usuário está autenticado.
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        return authState.User.Identity != null && authState.User.Identity.IsAuthenticated;
    }

    /// <summary>
    /// Obtém o token atual.
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("authToken");
    }

    /// <summary>
    /// Obtém o refresh token atual.
    /// </summary>
    public async Task<string?> GetRefreshTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>("refreshToken");
    }

    /// <summary>
    /// Obtém informações completas do token.
    /// </summary>
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
            ExpiresAt = expiration ?? DateTime.UtcNow.AddHours(-1),
            ExpiresIn = expiration.HasValue ? (int)(expiration.Value - DateTime.UtcNow).TotalSeconds : 0
        };
    }

    private async Task StoreTokensAndNotifyAsync(LoginResponse loginResponse, CancellationToken cancellationToken)
    {
        await TokenRefreshCoordinator.PersistTokensAsync(loginResponse, _localStorage, cancellationToken);
        await Task.Delay(50, cancellationToken);
        await ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated();
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return await ReadErrorMessageFromContentAsync(response.StatusCode, content, cancellationToken);
    }

    private static Task<string> ReadErrorMessageFromContentAsync(HttpStatusCode statusCode, string content, CancellationToken _)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return Task.FromResult(statusCode switch
            {
                HttpStatusCode.Unauthorized => "Credenciais inválidas.",
                HttpStatusCode.Conflict => "Este email já está em uso.",
                HttpStatusCode.BadRequest => "Dados inválidos.",
                _ => "Ocorreu um erro. Tente novamente."
            });
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            if (root.TryGetProperty("message", out var msgProp))
            {
                var text = msgProp.GetString();
                if (!string.IsNullOrEmpty(text))
                    return Task.FromResult(text);
            }

            if (root.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Array)
            {
                var parts = new List<string>();
                foreach (var e in errorsProp.EnumerateArray())
                {
                    if (e.ValueKind == JsonValueKind.String)
                    {
                        var s = e.GetString();
                        if (!string.IsNullOrEmpty(s))
                            parts.Add(s);
                    }
                }
                if (parts.Count > 0)
                    return Task.FromResult(string.Join(" ", parts));
            }
        }
        catch (JsonException)
        {
            // conteúdo não JSON
        }

        return Task.FromResult(content);
    }
}
