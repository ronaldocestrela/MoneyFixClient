# MoneyFix Client - Sistema de Configuração

## 🎯 Configuração de Ambiente

O MoneyFix Client agora suporta configurações separadas para desenvolvimento e produção com alternância automática baseada em variáveis de ambiente.

### 🔧 URLs Configuradas

| Ambiente | URL da API |
|----------|------------|
| **Desenvolvimento** | `http://localhost:5223` |
| **Produção** | `https://api.moneyfix.com.br` |

## 📁 Arquivos de Configuração

```
wwwroot/
├── appsettings.json              # Configuração de desenvolvimento
├── appsettings.Production.json   # Configuração de produção
└── appsettings.current.json      # Configuração ativa (gerada pelo script)
```

## 🛠️ Script de Gerenciamento

Use o script `environment-config.sh` para alternar entre ambientes:

```bash
# Configurar para desenvolvimento
./environment-config.sh dev

# Configurar para produção
./environment-config.sh prod

# Ver configuração atual
./environment-config.sh status

# Ajuda
./environment-config.sh help
```

## 🚀 Detecção Automática de Ambiente

O sistema detecta automaticamente o ambiente através das seguintes variáveis:

- `ASPNETCORE_ENVIRONMENT`
- `DOTNET_ENVIRONMENT`
- `ENVIRONMENT`

### Ordem de Prioridade:
1. Variáveis de ambiente
2. Arquivo `appsettings.current.json` (se existir)
3. **Padrão**: Desenvolvimento

## 🐳 Docker

### Desenvolvimento
```bash
# Usa configuração de desenvolvimento automaticamente
docker-compose -f docker-compose.dev.yml up -d
```

### Produção
```bash
# Usa configuração de produção automaticamente
docker-compose up -d
```

O Dockerfile automaticamente copia `appsettings.Production.json` como `appsettings.json` durante o build para produção.

## 📚 Estrutura de Configuração

### appsettings.json (Desenvolvimento)
```json
{
  "ApiSettings": {
    "DevelopmentBaseUrl": "http://localhost:5223",
    "ProductionBaseUrl": "https://api.moneyfix.com.br",
    "LoginEndpoint": "/api/login",
    "RegisterEndpoint": "/api/account/register"
  },
  "Environment": {
    "IsDevelopment": true
  }
}
```

### appsettings.Production.json
```json
{
  "ApiSettings": {
    "DevelopmentBaseUrl": "http://localhost:5223",
    "ProductionBaseUrl": "https://api.moneyfix.com.br",
    "LoginEndpoint": "/api/login",
    "RegisterEndpoint": "/api/account/register"
  },
  "Environment": {
    "IsDevelopment": false
  }
}
```

## 🔄 ConfigurationService

O `ConfigurationService` foi refatorado para suportar URLs duplas:

```csharp
public class ApiSettings
{
    public string DevelopmentBaseUrl { get; set; }
    public string ProductionBaseUrl { get; set; }
    
    public string GetBaseUrl(bool isDevelopment)
    {
        return isDevelopment ? DevelopmentBaseUrl : ProductionBaseUrl;
    }
}
```

## 🎭 Alternância Manual

Para forçar um ambiente específico:

```bash
# Desenvolvimento
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

# Produção
export ASPNETCORE_ENVIRONMENT=Production
export DOTNET_ENVIRONMENT=Production
```

## 🔍 Troubleshooting

### Verificar ambiente atual:
```bash
echo "ASPNETCORE_ENVIRONMENT: $ASPNETCORE_ENVIRONMENT"
echo "DOTNET_ENVIRONMENT: $DOTNET_ENVIRONMENT"
./environment-config.sh status
```

### Problemas de conexão:
1. Verifique se a API está rodando na URL correta
2. Confirme o ambiente com `./environment-config.sh status`
3. Verifique variáveis de ambiente
4. Para desenvolvimento, certifique-se que a API local está na porta 5223

### Reset de configuração:
```bash
# Remove configuração atual
rm -f wwwroot/appsettings.current.json

# Redefine para desenvolvimento
./environment-config.sh dev
```

## 🏷️ Versionamento

- **v1.4.0**: Sistema de configuração dual implementado
- **v1.3.x**: Sistema de registro de usuário
- **v1.2.x**: Dashboard e filtros
- **v1.1.x**: CRUD de categorias e transações
- **v1.0.x**: Sistema base de autenticação

## 📝 Próximos Passos

- [ ] Configuração de CORS para produção
- [ ] Certificados SSL para API de produção
- [ ] Health checks para diferentes ambientes
- [ ] Logs específicos por ambiente
