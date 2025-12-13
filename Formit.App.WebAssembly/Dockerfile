# ----------------------------------------------------
# 1. STAGE DE BUILD (Compilação do .NET SDK)
# ----------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Formit.sln .
COPY Formit.App.WebAssembly/ Formit.App.WebAssembly/
COPY Formit.Shared/ Formit.Shared/ 

RUN dotnet restore ./Formit.App.WebAssembly/Formit.App.WebAssembly.csproj

WORKDIR /src/Formit.App.WebAssembly

RUN dotnet publish "Formit.App.WebAssembly.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html


COPY Formit.App.WebAssembly/nginx.conf /etc/nginx/conf.d/default.conf

COPY --from=build /app/publish/wwwroot . 

EXPOSE 80
ENTRYPOINT ["nginx", "-g", "daemon off;"]