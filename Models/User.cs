namespace MoneyFixClient.Models;

/// <summary>
/// Modelo para representar um usuário autenticado
/// </summary>
public class User
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Informações do usuário extraídas do token JWT
/// </summary>
public class UserInfo
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public bool IsExpired => DateTime.UtcNow > ExpirationDate;
}

/// <summary>
/// Modelo para informações de token
/// </summary>
public class TokenInfo
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public int ExpiresIn { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsExpiringSoon => DateTime.UtcNow.AddMinutes(5) >= ExpiresAt;
}
