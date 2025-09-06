# MoneyFix Client - Configuração Docker

Este documento descreve como executar o MoneyFix Client usando Docker e Docker Compose.

## 📋 Pré-requisitos

- Docker 20.10+
- Docker Compose 1.29+

## 🚀 Execução Rápida

### Modo Desenvolvimento
```bash
# Usando script de gerenciamento
./docker-manage.sh dev

# Ou usando docker-compose diretamente
docker-compose -f docker-compose.dev.yml up -d
```

Aplicação disponível em: `http://localhost:5133`

### Modo Produção
```bash
# Usando script de gerenciamento
./docker-manage.sh prod

# Ou usando docker-compose diretamente
docker-compose up -d moneyfix-client
```

Aplicação disponível em: `http://localhost:5133`

## 🛠️ Script de Gerenciamento

O arquivo `docker-manage.sh` fornece comandos convenientes para gerenciar a aplicação:

```bash
# Construir a imagem
./docker-manage.sh build

# Iniciar em desenvolvimento
./docker-manage.sh dev

# Iniciar em produção
./docker-manage.sh prod

# Parar containers
./docker-manage.sh stop

# Reiniciar
./docker-manage.sh restart

# Ver logs
./docker-manage.sh logs
./docker-manage.sh logs -f  # Seguir logs em tempo real

# Limpar containers e imagens
./docker-manage.sh clean

# Ver status
./docker-manage.sh status

# Acessar shell do container
./docker-manage.sh shell

# Ajuda
./docker-manage.sh help
```

## 📁 Estrutura de Arquivos Docker

```
├── Dockerfile                 # Imagem principal da aplicação
├── docker-compose.yml         # Configuração para produção
├── docker-compose.dev.yml     # Configuração para desenvolvimento
├── .dockerignore              # Arquivos ignorados no build
├── docker-manage.sh           # Script de gerenciamento
└── DOCKER.md                  # Esta documentação
```

## 🔧 Configurações

### Portas

- **Desenvolvimento e Produção**: `5133:5133`

### Variáveis de Ambiente

| Variável | Valor Padrão | Descrição |
|----------|--------------|-----------|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Ambiente da aplicação |
| `ASPNETCORE_URLS` | `http://+:5133` | URL de binding da aplicação |

### Volumes

- `app-logs`: Logs da aplicação

## 🔍 Troubleshooting

### Verificar status dos containers
```bash
docker ps --filter "name=moneyfix"
```

### Ver logs específicos
```bash
docker logs moneyfix-client-dev
docker logs moneyfix-client
```

### Reconstruir imagem
```bash
docker build --no-cache -t moneyfix-client:latest .
```

### Limpar cache Docker
```bash
docker system prune -a
```

### Verificar recursos utilizados
```bash
docker stats --format "table {{.Name}}\t{{.CPUPerc}}\t{{.MemUsage}}\t{{.NetIO}}"
```

## 🔒 Segurança

A aplicação está configurada com:

- HTTPS pronto para uso em produção
- Configurações de CORS apropriadas
- Headers de segurança via ASP.NET Core

## 📈 Performance

### Otimizações incluídas:

- Build multi-stage para reduzir tamanho da imagem
- Aplicação Blazor WebAssembly otimizada
- Recursos estáticos servidos pelo Kestrel

### Métricas de build:

- Tamanho da imagem final: ~200MB (aspnet:9.0 + aplicação)
- Tempo de build: ~2-3 minutos
- Tempo de startup: ~10-15 segundos

## 🚀 Deploy em Produção

### Usando Docker Compose
```bash
# Clone o repositório
git clone <repository-url>
cd MoneyFixClient

# Construir e executar
./docker-manage.sh build
./docker-manage.sh prod
```

### Usando apenas Docker
```bash
# Construir
docker build -t moneyfix-client:latest .

# Executar
docker run -d \
  --name moneyfix-client \
  -p 5133:5133 \
  --restart unless-stopped \
  moneyfix-client:latest
```

## 🔄 CI/CD

Exemplo de pipeline para GitHub Actions:

```yaml
name: Build and Deploy
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build Docker image
        run: docker build -t moneyfix-client:latest .
      - name: Deploy
        run: |
          docker stop moneyfix-client || true
          docker rm moneyfix-client || true
          docker run -d --name moneyfix-client -p 5133:5133 moneyfix-client:latest
```
