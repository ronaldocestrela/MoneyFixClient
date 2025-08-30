using System.Text.Json;

namespace MoneyFixClient.Services;

/// <summary>
/// Serviço para gerenciar configurações da aplicação
/// </summary>
public class ConfigurationService
{
    private readonly HttpClient _httpClient;
    private AppSettings? _appSettings;

    public ConfigurationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Carrega as configurações da aplicação
    /// </summary>
    public async Task<AppSettings> GetAppSettingsAsync()
    {
        if (_appSettings == null)
        {
            try
            {
                var response = await _httpClient.GetStringAsync("appsettings.json");
                _appSettings = JsonSerializer.Deserialize<AppSettings>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? GetDefaultSettings();
            }
            catch
            {
                // Configurações padrão em caso de erro
                _appSettings = GetDefaultSettings();
            }
        }

        return _appSettings;
    }

    private static AppSettings GetDefaultSettings()
    {
        return new AppSettings
        {
            ApiSettings = new ApiSettings
            {
                BaseUrl = "http://localhost:5223",
                LoginEndpoint = "/api/login",
                Timeout = 30
            },
            Authentication = new AuthenticationSettings
            {
                TokenKey = "authToken",
                RedirectAfterLogin = "/dashboard",
                RedirectAfterLogout = "/login"
            },
            UI = new UISettings
            {
                AppName = "MoneyFix",
                Version = "1.0.0",
                Theme = "bootstrap"
            }
        };
    }
}

/// <summary>
/// Configurações da aplicação
/// </summary>
public class AppSettings
{
    public ApiSettings ApiSettings { get; set; } = new();
    public AuthenticationSettings Authentication { get; set; } = new();
    public UISettings UI { get; set; } = new();
}

/// <summary>
/// Configurações da API
/// </summary>
public class ApiSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string LoginEndpoint { get; set; } = string.Empty;
    public int Timeout { get; set; } = 30;
}

/// <summary>
/// Configurações de autenticação
/// </summary>
public class AuthenticationSettings
{
    public string TokenKey { get; set; } = string.Empty;
    public string RedirectAfterLogin { get; set; } = string.Empty;
    public string RedirectAfterLogout { get; set; } = string.Empty;
}

/// <summary>
/// Configurações da interface
/// </summary>
public class UISettings
{
    public string AppName { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Theme { get; set; } = string.Empty;
}
