using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MoneyFixClient.Providers;

/// <summary>
/// Provedor de estado de autenticação: JWT do localStorage e claims do token.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <inheritdoc />
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");

        if (string.IsNullOrEmpty(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        try
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token))
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

            var jwt = handler.ReadJwtToken(token);

            // Usar ValidTo do JWT (UTC). Evita falsos "expirado" por desserialização de DateTime no localStorage.
            if (jwt.ValidTo < DateTime.UtcNow)
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            var claims = new List<Claim>();

            foreach (var claim in jwt.Claims)
            {
                switch (claim.Type)
                {
                    case JwtRegisteredClaimNames.Sub:
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, claim.Value));
                        break;
                    case "email":
                        claims.Add(new Claim(ClaimTypes.Email, claim.Value));
                        break;
                    case "name":
                        claims.Add(new Claim(ClaimTypes.Name, claim.Value));
                        break;
                    default:
                        claims.Add(claim);
                        break;
                }
            }

            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                var sub = jwt.Claims.FirstOrDefault(c => c.Type is JwtRegisteredClaimNames.Sub or "sub");
                if (sub != null)
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, sub.Value));
            }

            if (!claims.Any(c => c.Type == ClaimTypes.Name))
            {
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (!string.IsNullOrEmpty(email))
                    claims.Add(new Claim(ClaimTypes.Name, email));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }
    }

    /// <summary>
    /// Notifica que o utilizador autenticou-se (tokens já gravados).
    /// </summary>
    public async Task MarkUserAsAuthenticated()
    {
        var authState = await GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    /// <summary>
    /// Notifica logout local.
    /// </summary>
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
    }
}
