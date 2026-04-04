using Microsoft.Extensions.Configuration;

namespace MoneyFixClient.Services;

/// <summary>
/// Resolve a URL base da API a partir da configuração (host WASM).
/// Deve incluir o segmento <c>/api</c> quando a API expõe identidade em <c>/api/identity/...</c>.
/// Com <see cref="HttpClient"/>, caminhos de pedido que começam com <c>/</c> ignoram o path da base; usar <c>identity/register</c> e não <c>/identity/register</c>.
/// </summary>
public static class ApiBaseUrlResolver
{
    /// <summary>
    /// Garante barra final para combinar corretamente com caminhos relativos do <see cref="HttpClient"/>.
    /// </summary>
    public static string Resolve(IConfiguration configuration)
    {
        var raw = configuration["BaseUrl"]
                  ?? configuration["ApiSettings:BaseUrl"]
                  ?? "http://localhost:5003/api";
        return raw.TrimEnd('/') + "/";
    }
}
