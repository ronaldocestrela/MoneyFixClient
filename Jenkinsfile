pipeline {
    agent any
    
    environment {
        DOTNET_VERSION = '9.0'
        APP_NAME = 'moneyfix-client'
        DOCKER_IMAGE = "${APP_NAME}:${BUILD_NUMBER}"
        DOCKER_REGISTRY = 'your-registry.com' // Substitua pela sua registry
    }
    
    stages {
        stage('Checkout') {
            steps {
                git branch: 'developer',
                    url: 'https://github.com/ronaldocestrela/MoneyFixClient.git'
            }
        }
        
        stage('Build .NET') {
            steps {
                script {
                    // Instala .NET SDK se necessário
                    sh '''
                        if ! command -v dotnet &> /dev/null; then
                            echo "Installing .NET SDK..."
                            wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
                            chmod +x dotnet-install.sh
                            ./dotnet-install.sh --version ${DOTNET_VERSION} --install-dir /usr/share/dotnet
                            export PATH=/usr/share/dotnet:$PATH
                        fi
                    '''
                }
                
                // Build da aplicação
                sh 'dotnet restore'
                sh 'dotnet build -c Release --no-restore'
                sh 'dotnet publish -c Release -o ./publish --no-build'
            }
        }
        
        stage('Docker Build') {
            steps {
                script {
                    // Build da imagem Docker
                    sh "docker build -t ${DOCKER_IMAGE} ."
                    sh "docker tag ${DOCKER_IMAGE} ${APP_NAME}:latest"
                }
            }
        }
        
        stage('Deploy') {
            steps {
                script {
                    // Para o container anterior se existir
                    sh '''
                        docker stop ${APP_NAME} || true
                        docker rm ${APP_NAME} || true
                    '''
                    
                    // Executa o novo container
                    sh '''
                        docker run -d \
                            --name ${APP_NAME} \
                            -p 5133:5133 \
                            --restart unless-stopped \
                            ${DOCKER_IMAGE}
                    '''
                }
            }
        }
        
        stage('Health Check') {
            steps {
                script {
                    // Aguarda o container iniciar
                    sleep(time: 10, unit: 'SECONDS')
                    
                    // Verifica se a aplicação está respondendo
                    sh '''
                        for i in {1..30}; do
                            if curl -f http://localhost:5133/ > /dev/null 2>&1; then
                                echo "Application is healthy!"
                                exit 0
                            fi
                            echo "Waiting for application to start... ($i/30)"
                            sleep 2
                        done
                        echo "Health check failed!"
                        exit 1
                    '''
                }
            }
        }
    }
    
    post {
        always {
            // Limpeza de imagens antigas
            sh '''
                docker image prune -f --filter "until=24h"
                docker system prune -f --volumes --filter "until=24h"
            '''
        }
        
        success {
            echo 'Deploy realizado com sucesso!'
            // Aqui você pode adicionar notificações de sucesso
        }
        
        failure {
            echo 'Deploy falhou!'
            // Para o container em caso de falha
            sh 'docker stop ${APP_NAME} || true'
            // Aqui você pode adicionar notificações de falha
        }
    }
}
