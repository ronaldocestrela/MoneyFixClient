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

## 📋 Versionamento

Este projeto segue o padrão [Semantic Versioning (SemVer)](https://semver.org/lang/pt-BR/) para controle de versões.

### Formato de Versionamento
`MAJOR.MINOR.PATCH`

- **MAJOR**: Mudanças incompatíveis na API
- **MINOR**: Funcionalidades adicionadas de forma compatível
- **PATCH**: Correções de bugs compatíveis

### Convenção de Commits

Para manter um histórico claro, utilize as seguintes convenções:

#### Tipos de Commit
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Apenas mudanças na documentação
- `style`: Mudanças que não afetam o significado do código (espaços, formatação, etc.)
- `refactor`: Mudança de código que não corrige bug nem adiciona funcionalidade
- `perf`: Mudança de código que melhora performance
- `test`: Adição ou correção de testes
- `chore`: Mudanças no processo de build ou ferramentas auxiliares

#### Exemplos de Commits
```bash
git commit -m "feat: adiciona sistema de categorias de transações"
git commit -m "fix: corrige erro de autenticação no localStorage"
git commit -m "docs: atualiza README com instruções de instalação"
git commit -m "refactor: reorganiza estrutura de serviços"
git commit -m "style: aplica formatação consistente no código"
git commit -m "perf: otimiza carregamento de dados na dashboard"
git commit -m "test: adiciona testes unitários para AuthService"
git commit -m "chore: atualiza dependências do projeto"
```

### Workflow de Branches

#### Branch Principal
- `main`: Branch de produção, sempre estável

#### Branches de Desenvolvimento
- `develop`: Branch de desenvolvimento principal
- `feature/nome-da-feature`: Novas funcionalidades
- `hotfix/nome-do-fix`: Correções urgentes
- `release/x.y.z`: Preparação para release

#### Fluxo de Trabalho
```bash
# 1. Criar nova feature
git checkout -b feature/nova-funcionalidade

# 2. Desenvolver e commitar
git add .
git commit -m "feat: adiciona nova funcionalidade"

# 3. Push da branch
git push origin feature/nova-funcionalidade

# 4. Criar Pull Request para develop
# 5. Após review e aprovação, merge para develop
# 6. Para release, merge develop -> main com tag de versão
```

### Tags de Versão

#### Criando uma Nova Versão
```bash
# 1. Fazer merge da develop para main
git checkout main
git merge develop

# 2. Criar tag com a versão
git tag -a v1.2.0 -m "Release v1.2.0 - Adiciona sistema de categorias"

# 3. Push da tag
git push origin v1.2.0
git push origin main
```

#### Formato de Tags
- `v1.0.0` - Release principal
- `v1.1.0` - Nova funcionalidade
- `v1.1.1` - Correção de bug
- `v2.0.0` - Breaking changes

### Changelog

Mantenha um arquivo `CHANGELOG.md` atualizado com:
- Data de release
- Versão
- Mudanças adicionadas, modificadas e removidas
- Breaking changes
- Correções de bugs

#### Exemplo de Entrada no Changelog
```markdown
## [1.2.0] - 2025-08-30

### Adicionado
- Sistema de categorias de transações
- Filtros avançados na dashboard
- Exportação de dados em CSV

### Modificado
- Melhorias na interface de login
- Otimização do carregamento de dados

### Corrigido
- Erro de autenticação no localStorage
- Bug na validação de formulários

### Removido
- Dependência desnecessária no package.json
```

### Versionamento Automático

Para automatizar o versionamento, considere usar:
- **GitVersion**: Gera versões baseadas no Git
- **GitHub Actions**: CI/CD com versionamento automático
- **Conventional Commits**: Padronização que permite automação

```yml
# Exemplo de GitHub Action para versionamento
name: Release
on:
  push:
    branches: [ main ]
jobs:
  release:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v2
    - name: Semantic Release
      uses: cycjimmy/semantic-release-action@v2
```

---

**MoneyFix Client** - Gerencie suas finanças com tecnologia moderna! 💰
