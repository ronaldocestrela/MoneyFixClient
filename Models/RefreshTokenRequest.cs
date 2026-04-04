using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Corpo da requisição POST /identity/refresh
/// </summary>
public class RefreshTokenRequest
{
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}
