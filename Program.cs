using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using MoneyFixClient;
using MoneyFixClient.Services;
using MoneyFixClient.Providers;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuração do LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Configuração da Autenticação
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Configuração dos Serviços
builder.Services.AddScoped<TokenRefreshCoordinator>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<AuthHeaderHandler>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<AccountService>();

// Configuração do HttpClient com AuthHeaderHandler
var apiBaseUrl = ApiBaseUrlResolver.Resolve(builder.Configuration);
builder.Services.AddScoped(sp =>
{
    var authHeaderHandler = sp.GetRequiredService<AuthHeaderHandler>();
    authHeaderHandler.InnerHandler = new HttpClientHandler();

    var httpClient = new HttpClient(authHeaderHandler)
    {
        BaseAddress = new Uri(apiBaseUrl)
    };

    return httpClient;
});

await builder.Build().RunAsync();
