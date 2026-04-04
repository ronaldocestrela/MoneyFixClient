using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para solicitação de registro de usuário (alinhado a POST /identity/register).
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário (complexidade conforme API)
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Senha deve ter entre 8 e 100 caracteres")]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
        ErrorMessage = "Senha deve ter maiúscula, minúscula, número e caractere especial")]
    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha
    /// </summary>
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "Senhas não conferem")]
    [JsonIgnore]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo do usuário
    /// </summary>
    [Required(ErrorMessage = "Nome completo é obrigatório")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome completo deve ter entre 2 e 200 caracteres")]
    [JsonPropertyName("fullName")]
    public string FullName { get; set; } = string.Empty;
}
