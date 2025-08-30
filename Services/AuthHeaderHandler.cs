using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace MoneyFixClient.Services;

/// <summary>
/// Handler para adicionar automaticamente o token JWT no cabeçalho Authorization das requisições HTTP
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ILocalStorageService _localStorage;

    public AuthHeaderHandler(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        // Obtém o token do localStorage
        var token = await _localStorage.GetItemAsync<string>("authToken");

        // Se existe token, adiciona no cabeçalho Authorization
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
