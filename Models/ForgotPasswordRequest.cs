using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Corpo da requisição POST /identity/forgot-password
/// </summary>
public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}
