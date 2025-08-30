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
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<AuthHeaderHandler>();

// Configuração do HttpClient com AuthHeaderHandler
builder.Services.AddScoped(sp =>
{
    var authHeaderHandler = sp.GetRequiredService<AuthHeaderHandler>();
    authHeaderHandler.InnerHandler = new HttpClientHandler();
    
    var httpClient = new HttpClient(authHeaderHandler)
    {
        // Configure aqui a URL base da sua API
        BaseAddress = new Uri("http://localhost:5223") // URL da API MoneyFix
    };
    
    return httpClient;
});

await builder.Build().RunAsync();
