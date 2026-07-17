# Acesse https://aka.ms/customizecontainer para saber como personalizar
# o contêiner de depuração e como o Visual Studio utiliza este Dockerfile.

# Usado pelo Visual Studio durante a depuração em Fast Mode.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER $APP_UID
WORKDIR /app

EXPOSE 8080
EXPOSE 8081


# Compila a aplicação.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /workspace

COPY ["src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj", "src/CorporateServiceDesk.Api/"]
COPY ["src/CorporateServiceDesk.Application/CorporateServiceDesk.Application.csproj", "src/CorporateServiceDesk.Application/"]
COPY ["src/CorporateServiceDesk.Domain/CorporateServiceDesk.Domain.csproj", "src/CorporateServiceDesk.Domain/"]
COPY ["src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj", "src/CorporateServiceDesk.Infrastructure/"]

RUN dotnet restore "./src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj"

COPY . .

WORKDIR "/workspace/src/CorporateServiceDesk.Api"

RUN dotnet build "./CorporateServiceDesk.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build


# Publica a aplicação.
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./CorporateServiceDesk.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# Imagem final utilizada em produção ou no modo normal do Visual Studio.
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CorporateServiceDesk.Api.dll"]