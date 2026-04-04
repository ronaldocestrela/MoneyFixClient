using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Resposta de POST /identity/forgot-password (MVP: token retornado na resposta).
/// </summary>
public class ForgotPasswordResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("resetToken")]
    public string ResetToken { get; set; } = string.Empty;
}
