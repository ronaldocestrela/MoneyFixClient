using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace MoneyFixClient.Providers;

/// <summary>
/// Provedor de estado de autenticação customizado para JWT
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Obtém o estado atual de autenticação
    /// </summary>
    /// <returns>Estado de autenticação</returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        Console.WriteLine($"AuthStateProvider: Token encontrado? {!string.IsNullOrEmpty(token)}");
        
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"AuthStateProvider: Token primeiros 20 chars: {token[..Math.Min(20, token.Length)]}...");
        }

        if (string.IsNullOrEmpty(token))
        {
            Console.WriteLine("AuthStateProvider: Token vazio, retornando usuário anônimo");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Verifica se o token expirou baseado na data salva no localStorage
        var tokenExpiration = await _localStorage.GetItemAsync<DateTime?>("tokenExpiration");
        var isExpired = tokenExpiration.HasValue && tokenExpiration.Value < DateTime.UtcNow;
        Console.WriteLine($"AuthStateProvider: Token expira em: {tokenExpiration:O}, Agora: {DateTime.UtcNow:O}");
        Console.WriteLine($"AuthStateProvider: Token expirado por data salva? {isExpired}");
        
        if (isExpired)
        {
            Console.WriteLine("AuthStateProvider: Token expirado, retornando usuário anônimo");
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Como não é um JWT válido, cria claims básicas para autenticação
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "User"),
            new Claim(ClaimTypes.NameIdentifier, "user-id"),
            new Claim("token", token[..Math.Min(10, token.Length)]) // Primeiros 10 chars do token
        };
        
        Console.WriteLine($"AuthStateProvider: Claims criadas: {claims.Count}");
        
        var identity = new ClaimsIdentity(claims, "custom-token");
        var user = new ClaimsPrincipal(identity);
        
        Console.WriteLine($"AuthStateProvider: Usuário autenticado com token customizado");
        Console.WriteLine($"AuthStateProvider: Identity.IsAuthenticated: {identity.IsAuthenticated}");
        return new AuthenticationState(user);
    }

    /// <summary>
    /// Marca o usuário como autenticado
    /// </summary>
    public async Task MarkUserAsAuthenticated()
    {
        Console.WriteLine("AuthStateProvider: MarkUserAsAuthenticated chamado");
        var authState = await GetAuthenticationStateAsync();
        Console.WriteLine($"AuthStateProvider: Notificando mudança de estado. Usuário autenticado: {authState.User.Identity?.IsAuthenticated}");
        NotifyAuthenticationStateChanged(Task.FromResult(authState));
    }

    /// <summary>
    /// Marca o usuário como deslogado
    /// </summary>
    public void MarkUserAsLoggedOut()
    {
        var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
        var authState = Task.FromResult(new AuthenticationState(anonymousUser));
        NotifyAuthenticationStateChanged(authState);
    }

}
