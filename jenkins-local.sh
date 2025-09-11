#!/bin/bash

# Script para executar pipeline Jenkins localmente

set -e

# Cores para output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
BLUE='\033[0;34m'
NC='\033[0m'

print_message() {
    echo -e "${2}${1}${NC}"
}

print_message "🚀 Executando Pipeline Jenkins Localmente" $BLUE

# Stage 1: Checkout (já estamos no diretório)
print_message "📥 Stage: Checkout" $YELLOW
echo "✅ Código já disponível localmente"

# Stage 2: Build .NET
print_message "🔨 Stage: Build .NET" $YELLOW
echo "Restaurando dependências..."
dotnet restore

echo "Building aplicação..."
dotnet build -c Release --no-restore

echo "Publicando aplicação..."
dotnet publish -c Release -o ./publish --no-build

print_message "✅ Build .NET concluído!" $GREEN

# Stage 3: Docker Build
print_message "🐳 Stage: Docker Build" $YELLOW
echo "Construindo imagem Docker..."
docker build -t moneyfix-client:latest .

print_message "✅ Docker Build concluído!" $GREEN

# Stage 4: Deploy
print_message "🚀 Stage: Deploy" $YELLOW
echo "Parando container anterior..."
docker stop moneyfix-client 2>/dev/null || true
docker rm moneyfix-client 2>/dev/null || true

echo "Iniciando novo container..."
docker run -d \
    --name moneyfix-client \
    -p 5133:5133 \
    --restart unless-stopped \
    moneyfix-client:latest

print_message "✅ Deploy concluído!" $GREEN

# Stage 5: Health Check
print_message "🔍 Stage: Health Check" $YELLOW
echo "Aguardando aplicação iniciar..."
sleep 10

for i in {1..30}; do
    if curl -f http://localhost:5133/ > /dev/null 2>&1; then
        print_message "✅ Health Check passou! Aplicação rodando em http://localhost:5133" $GREEN
        break
    fi
    echo "Aguardando... ($i/30)"
    sleep 2
    
    if [ $i -eq 30 ]; then
        print_message "❌ Health Check falhou!" $RED
        docker logs moneyfix-client --tail 20
        exit 1
    fi
done

# Post: Limpeza
print_message "🧹 Limpeza de imagens antigas" $YELLOW
docker image prune -f --filter "until=24h" > /dev/null 2>&1 || true

print_message "🎉 Pipeline concluído com sucesso!" $GREEN
print_message "📱 Aplicação disponível em: http://localhost:5133" $BLUE
