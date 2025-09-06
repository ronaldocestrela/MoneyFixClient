using System.ComponentModel.DataAnnotations;

namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para solicitação de registro de usuário
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Email do usuário
    /// </summary>
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email deve ter um formato válido")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Senha do usuário
    /// </summary>
    [Required(ErrorMessage = "Senha é obrigatória")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Confirmação da senha
    /// </summary>
    [Required(ErrorMessage = "Confirmação de senha é obrigatória")]
    [Compare("Password", ErrorMessage = "Senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// Primeiro nome do usuário
    /// </summary>
    [Required(ErrorMessage = "Primeiro nome é obrigatório")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Primeiro nome deve ter entre 2 e 50 caracteres")]
    public string UserFirstName { get; set; } = string.Empty;

    /// <summary>
    /// Último nome do usuário
    /// </summary>
    [Required(ErrorMessage = "Último nome é obrigatório")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Último nome deve ter entre 2 e 50 caracteres")]
    public string UserLastName { get; set; } = string.Empty;
}

/// <summary>
/// Modelo para resposta de registro
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// Indica se o registro foi bem-sucedido
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem de retorno
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// ID do usuário criado (se sucesso)
    /// </summary>
    public string? UserId { get; set; }
}
