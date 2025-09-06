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

# Use a imagem oficial do ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Define o diretório de trabalho
WORKDIR /app

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Define a variável de ambiente para a porta
ENV ASPNETCORE_URLS=http://+:5133

# Expõe a porta 5133
EXPOSE 5133

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "MoneyFixClient.dll"]
