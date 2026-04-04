using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using MoneyFixClient.Models;
using MoneyFixClient.Providers;
using System.Net.Http.Json;
using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Renova tokens sem usar o <see cref="HttpClient"/> com <see cref="AuthHeaderHandler"/> (evita dependência circular e envio de Bearer em /identity/refresh).
/// </summary>
public class TokenRefreshCoordinator
{
    private readonly ILocalStorageService _localStorage;
    private readonly IConfiguration _configuration;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TokenRefreshCoordinator(
        ILocalStorageService localStorage,
        IConfiguration configuration,
        AuthenticationStateProvider authenticationStateProvider)
    {
        _localStorage = localStorage;
        _configuration = configuration;
        _authenticationStateProvider = authenticationStateProvider;
    }

    /// <summary>
    /// Tenta renovar o access token com o refresh token armazenado.
    /// </summary>
    /// <returns>True se novos tokens foram gravados.</returns>
    public async Task<bool> TryRefreshAsync(CancellationToken cancellationToken = default)
    {
        var refresh = await _localStorage.GetItemAsync<string>("refreshToken", cancellationToken);
        if (string.IsNullOrEmpty(refresh))
            return false;

        var baseUrl = ApiBaseUrlResolver.Resolve(_configuration);
        using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

        var response = await client.PostAsJsonAsync(
            "identity/refresh",
            new RefreshTokenRequest { RefreshToken = refresh },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, JsonOptions);
        if (loginResponse == null || string.IsNullOrEmpty(loginResponse.AccessToken))
            return false;

        await PersistTokensAsync(loginResponse, _localStorage, cancellationToken);
        await ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated();
        return true;
    }

    internal static async Task PersistTokensAsync(
        LoginResponse loginResponse,
        ILocalStorageService localStorage,
        CancellationToken cancellationToken = default)
    {
        var expiresAt = DateTime.UtcNow.AddSeconds(loginResponse.ExpiresIn);
        await localStorage.SetItemAsync("authToken", loginResponse.AccessToken, cancellationToken);
        await localStorage.SetItemAsync("refreshToken", loginResponse.RefreshToken, cancellationToken);
        await localStorage.SetItemAsync("tokenExpiration", expiresAt, cancellationToken);
    }
}
