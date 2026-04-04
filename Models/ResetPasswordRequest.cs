using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Corpo da requisição POST /identity/reset-password
/// </summary>
public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Token é obrigatório")]
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nova senha é obrigatória")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Senha deve ter entre 8 e 100 caracteres")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        ErrorMessage = "Senha deve ter maiúscula, minúscula, número e caractere especial")]
    [JsonPropertyName("newPassword")]
    public string NewPassword { get; set; } = string.Empty;
}
