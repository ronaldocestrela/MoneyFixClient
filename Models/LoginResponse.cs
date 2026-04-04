using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para resposta de login da API MoneyFix
/// </summary>
public class LoginResponse
{
    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
    
    // Propriedades auxiliares para compatibilidade
    public bool Success => !string.IsNullOrEmpty(AccessToken);
    public string Message { get; set; } = string.Empty;
    public string Token => AccessToken; // Para compatibilidade com código existente
    public DateTime Expiration => DateTime.UtcNow.AddSeconds(ExpiresIn);
}
