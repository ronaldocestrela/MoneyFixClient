using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace MoneyFixClient.Services;

/// <summary>
/// Adiciona Bearer ao pedido e renova o access token quando expira ou está prestes a expirar (1 min).
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;
    private readonly TokenRefreshCoordinator _tokenRefreshCoordinator;

    public AuthHeaderHandler(
        ILocalStorageService localStorage,
        TokenRefreshCoordinator tokenRefreshCoordinator)
    {
        _localStorage = localStorage;
        _tokenRefreshCoordinator = tokenRefreshCoordinator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await EnsureFreshAccessTokenAsync(cancellationToken);

        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }

    private async Task EnsureFreshAccessTokenAsync(CancellationToken cancellationToken)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken", cancellationToken);
        if (string.IsNullOrEmpty(token))
            return;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return;

            var jwt = handler.ReadJwtToken(token);
            // Renovar se expira em ≤ 1 minuto (mesma regra, mas com ValidTo UTC do JWT)
            if (jwt.ValidTo > DateTime.UtcNow.AddMinutes(1))
                return;
        }
        catch
        {
            return;
        }

        await _tokenRefreshCoordinator.TryRefreshAsync(cancellationToken);
    }
}
