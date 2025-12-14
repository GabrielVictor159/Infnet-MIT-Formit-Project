# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8084


# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Formit.Api/Formit.Api.csproj", "Formit.Api/"]
COPY ["Formit.Application/Formit.Application.csproj", "Formit.Application/"]
COPY ["Formit.Infraestructure/Formit.Infraestructure.csproj", "Formit.Infraestructure/"]
COPY ["Formit.Domain/Formit.Domain.csproj", "Formit.Domain/"]
COPY ["Formit.Infraestructure.IoC/Formit.Infraestructure.IoC.csproj", "Formit.Infraestructure.IoC/"]
RUN dotnet restore "./Formit.Api/Formit.Api.csproj"
COPY . .
WORKDIR "/src/Formit.Api"
RUN dotnet build "./Formit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Formit.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Formit.Api.dll", "--urls", "http://0.0.0.0:8084"]