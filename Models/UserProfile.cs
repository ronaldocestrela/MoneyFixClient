using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Perfil do usuário autenticado (GET /identity/me).
/// </summary>
public class UserProfile
{
    /// <summary>
    /// Identificador do usuário
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Email
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo
    /// </summary>
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação (UTC)
    /// </summary>
    [JsonPropertyName("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; }

    /// <summary>
    /// Último login (UTC)
    /// </summary>
    [JsonPropertyName("lastLoginUtc")]
    public DateTime LastLoginUtc { get; set; }

    /// <summary>
    /// Indica se a conta está ativa
    /// </summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
