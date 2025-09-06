#!/bin/bash

# Script para gerenciar containers Docker do MoneyFix Client

set -e

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para imprimir mensagens coloridas
print_message() {
    echo -e "${2}${1}${NC}"
}

# Função para mostrar uso
show_usage() {
    echo "Uso: ./docker-manage.sh [COMANDO] [OPÇÕES]"
    echo ""
    echo "Comandos disponíveis:"
    echo "  build       - Constrói a imagem Docker"
    echo "  dev         - Inicia em modo desenvolvimento"
    echo "  prod        - Inicia em modo produção"
    echo "  stop        - Para todos os containers"
    echo "  restart     - Reinicia os containers"
    echo "  logs        - Mostra logs dos containers"
    echo "  clean       - Remove containers e imagens"
    echo "  status      - Mostra status dos containers"
    echo "  shell       - Acessa shell do container"
    echo ""
    echo "Exemplos:"
    echo "  ./docker-manage.sh build"
    echo "  ./docker-manage.sh dev"
    echo "  ./docker-manage.sh logs -f"
}

# Função para construir a imagem
build_image() {
    print_message "🏗️ Construindo imagem Docker..." $BLUE
    docker build -t moneyfix-client:latest .
    print_message "✅ Imagem construída com sucesso!" $GREEN
}

# Função para iniciar em modo desenvolvimento
start_dev() {
    print_message "🚀 Iniciando em modo desenvolvimento..." $BLUE
    docker-compose -f docker-compose.dev.yml up -d
    print_message "✅ Aplicação disponível em: http://localhost:5133" $GREEN
}

# Função para iniciar em modo produção
start_prod() {
    print_message "🚀 Iniciando em modo produção..." $BLUE
    docker-compose up -d moneyfix-client
    print_message "✅ Aplicação disponível em: http://localhost:5133" $GREEN
}

# Função para parar containers
stop_containers() {
    print_message "⏹️ Parando containers..." $YELLOW
    docker-compose -f docker-compose.yml down
    docker-compose -f docker-compose.dev.yml down
    print_message "✅ Containers parados!" $GREEN
}

# Função para reiniciar
restart_containers() {
    print_message "🔄 Reiniciando containers..." $BLUE
    stop_containers
    sleep 2
    start_dev
}

# Função para mostrar logs
show_logs() {
    if [ "$2" = "-f" ]; then
        docker-compose logs -f
    else
        docker-compose logs --tail=50
    fi
}

# Função para limpeza
clean_all() {
    print_message "🧹 Limpando containers e imagens..." $YELLOW
    docker-compose -f docker-compose.yml down --volumes --remove-orphans
    docker-compose -f docker-compose.dev.yml down --volumes --remove-orphans
    docker system prune -f
    docker image rm moneyfix-client:latest 2>/dev/null || true
    print_message "✅ Limpeza concluída!" $GREEN
}

# Função para mostrar status
show_status() {
    print_message "📊 Status dos containers:" $BLUE
    docker ps --filter "name=moneyfix" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
}

# Função para acessar shell
access_shell() {
    print_message "🐚 Acessando shell do container..." $BLUE
    docker exec -it moneyfix-client-dev /bin/bash
}

# Verifica se Docker está instalado
if ! command -v docker &> /dev/null; then
    print_message "❌ Docker não está instalado!" $RED
    exit 1
fi

# Verifica se Docker Compose está instalado
if ! command -v docker-compose &> /dev/null; then
    print_message "❌ Docker Compose não está instalado!" $RED
    exit 1
fi

# Main
case "${1:-}" in
    "build")
        build_image
        ;;
    "dev")
        start_dev
        ;;
    "prod")
        start_prod
        ;;
    "stop")
        stop_containers
        ;;
    "restart")
        restart_containers
        ;;
    "logs")
        show_logs $@
        ;;
    "clean")
        clean_all
        ;;
    "status")
        show_status
        ;;
    "shell")
        access_shell
        ;;
    "help"|"-h"|"--help")
        show_usage
        ;;
    *)
        print_message "❌ Comando inválido: ${1:-}" $RED
        echo ""
        show_usage
        exit 1
        ;;
esac
