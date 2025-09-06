# Use a imagem oficial do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Define o diretório de trabalho
WORKDIR /src

# Copia o arquivo de projeto
COPY MoneyFixClient.csproj .

# Restaura as dependências
RUN dotnet restore

# Copia todo o código fonte
COPY . .

# Faz o build da aplicação em modo Release
RUN dotnet build -c Release -o /app/build

# Publica a aplicação
RUN dotnet publish -c Release -o /app/publish

# Use a imagem oficial do Nginx para servir a aplicação
FROM nginx:alpine AS final

# Remove a configuração padrão do Nginx
RUN rm -rf /usr/share/nginx/html/*

# Copia os arquivos publicados para o diretório do Nginx
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

# Cria configuração do Nginx para SPA
RUN echo 'server { \
    listen 5133; \
    server_name localhost; \
    root /usr/share/nginx/html; \
    index index.html; \
    location / { \
        try_files $uri $uri/ /index.html; \
    } \
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot|dll|wasm|dat)$ { \
        expires 1y; \
        add_header Cache-Control "public, immutable"; \
        access_log off; \
    } \
    location ~* \.wasm$ { \
        add_header Content-Type application/wasm; \
    } \
    gzip on; \
    gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript application/wasm; \
}' > /etc/nginx/conf.d/default.conf

# Remove a configuração padrão
RUN rm -f /etc/nginx/conf.d/default.conf.bak

# Expõe a porta 5133
EXPOSE 5133

# Comando para iniciar o Nginx
CMD ["nginx", "-g", "daemon off;"]
