# MoneyFix Client

MoneyFixClient é uma aplicação Blazor WebAssembly para gerenciamento financeiro pessoal que consome uma API externa com autenticação JWT.

## 🚀 Características

- **Framework**: Blazor WebAssembly (.NET 9)
- **Autenticação**: JWT (JSON Web Token)
- **UI**: Bootstrap 5 + FontAwesome
- **Armazenamento**: LocalStorage para persistência do token
- **Arquitetura**: Clean Architecture com separação de responsabilidades

## 📋 Funcionalidades

### ✅ Implementadas
- Sistema de autenticação com JWT
- Login/Logout de usuários
- Dashboard com overview financeiro
- Proteção de rotas
- Layout responsivo
- Gerenciamento de estado de autenticação
- Interceptação automática de requisições HTTP com token

### 🔄 Em Desenvolvimento
- Gestão de transações
- Categorização de gastos
- Relatórios financeiros
- Integração completa com API

## 🏗️ Estrutura do Projeto

```
MoneyFixClient/
├── Models/              # Modelos de dados
│   ├── LoginRequest.cs
│   └── LoginResponse.cs
├── Services/            # Serviços de negócio
│   ├── AuthService.cs
│   └── AuthHeaderHandler.cs
├── Providers/           # Provedores customizados
│   └── CustomAuthenticationStateProvider.cs
├── Pages/               # Páginas da aplicação
│   ├── Home.razor
│   ├── Login.razor
│   └── Dashboard.razor
├── Layout/              # Componentes de layout
│   ├── MainLayout.razor
│   ├── NavMenu.razor
│   └── RedirectToLogin.razor
└── wwwroot/            # Arquivos estáticos
```

## ⚙️ Configuração

### Pré-requisitos
- .NET 9 SDK
- API MoneyFix rodando (para autenticação)

### Configuração da API
Edite o arquivo `Program.cs` e ajuste a URL da API:

```csharp
BaseAddress = new Uri("http://localhost:5223") // URL da API MoneyFix
```

### Pacotes Utilizados
- `Blazored.LocalStorage` - Gerenciamento do localStorage
- `Microsoft.AspNetCore.Components.Authorization` - Sistema de autorização
- `System.IdentityModel.Tokens.Jwt` - Manipulação de tokens JWT

## 🚀 Como Executar

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd MoneyFix/MoneyFixClient
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Executar a aplicação**
   ```bash
   dotnet run
   ```

4. **Acessar no navegador**
   ```
   https://localhost:5001
   ```

## 🔧 Desenvolvimento

### Estrutura de Autenticação

#### AuthService
Responsável por:
- Login/logout de usuários
- Gerenciamento do token JWT
- Validação de expiração do token

#### CustomAuthenticationStateProvider
- Provê o estado de autenticação para toda a aplicação
- Extrai claims do token JWT
- Notifica mudanças de estado

#### AuthHeaderHandler
- Intercepta requisições HTTP
- Adiciona automaticamente o token Authorization
- Funciona como middleware para todas as chamadas à API

### Páginas

#### Login (`/login`)
- Formulário de autenticação
- Validação de campos
- Feedback de erros
- Redirecionamento automático após login

#### Dashboard (`/dashboard`)
- Área protegida (requer autenticação)
- Overview financeiro
- Cards com estatísticas
- Tabela de últimas transações

#### Home (`/`)
- Página inicial pública
- Apresentação do sistema
- Redirecionamento inteligente baseado no estado de autenticação

## 🔒 Segurança

- Tokens JWT armazenados em localStorage
- Verificação automática de expiração
- Proteção de rotas com `[Authorize]`
- Logout automático em caso de token inválido
- Headers de autorização adicionados automaticamente

## 📱 Responsividade

A aplicação é totalmente responsiva, utilizando:
- Bootstrap 5 para layout
- FontAwesome para ícones
- Design mobile-first
- Navegação adaptativa

## 🎯 Próximos Passos

1. **Integração com API**
   - Implementar chamadas reais para endpoints de transações
   - Adicionar tratamento de erros da API
   - Implementar refresh de token

2. **Funcionalidades Avançadas**
   - Gestão de categorias
   - Relatórios e gráficos
   - Filtros e pesquisas
   - Export de dados

3. **Melhorias de UX**
   - Loading states
   - Notificações toast
   - Confirmações de ações
   - Modo escuro

## 🐛 Troubleshooting

### Erro de CORS
Certifique-se de que a API está configurada para aceitar requisições do cliente Blazor.

### Token não encontrado
Verifique se a API está retornando o token no formato correto na resposta de login.

### Páginas não carregam
Verifique se todos os namespaces estão importados no `_Imports.razor`.

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

## 🤝 Contribuição

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

---

**MoneyFix Client** - Gerencie suas finanças com tecnologia moderna! 💰
