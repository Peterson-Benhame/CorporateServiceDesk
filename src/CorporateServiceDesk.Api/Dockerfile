# Imagem base da aplicação.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER $APP_UID
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

# Imagem utilizada para restore e compilação.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /workspace

# Arquivo responsável pelo gerenciamento central das versões.
COPY ["Directory.Packages.props", "./"]

# Projetos e arquivos lock utilizados pelo restore.
COPY ["src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj", "src/CorporateServiceDesk.Api/"]
COPY ["src/CorporateServiceDesk.Api/packages.lock.json", "src/CorporateServiceDesk.Api/"]

COPY ["src/CorporateServiceDesk.Application/CorporateServiceDesk.Application.csproj", "src/CorporateServiceDesk.Application/"]
COPY ["src/CorporateServiceDesk.Application/packages.lock.json", "src/CorporateServiceDesk.Application/"]

COPY ["src/CorporateServiceDesk.Domain/CorporateServiceDesk.Domain.csproj", "src/CorporateServiceDesk.Domain/"]
COPY ["src/CorporateServiceDesk.Domain/packages.lock.json", "src/CorporateServiceDesk.Domain/"]

COPY ["src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj", "src/CorporateServiceDesk.Infrastructure/"]
COPY ["src/CorporateServiceDesk.Infrastructure/packages.lock.json", "src/CorporateServiceDesk.Infrastructure/"]

COPY [".config/dotnet-tools.json", ".config/dotnet-tools.json"]

RUN dotnet restore "./src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj" --locked-mode
RUN dotnet tool restore --tool-manifest "/workspace/.config/dotnet-tools.json"

COPY . .

WORKDIR "/workspace/src/CorporateServiceDesk.Api"

RUN dotnet build "./CorporateServiceDesk.Api.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --output /app/build \
    --no-restore

# Publicação da API.
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./CorporateServiceDesk.Api.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

# Geração do migration bundle.
FROM build AS migrations

ARG BUILD_CONFIGURATION=Release

# Usada somente para criar o DbContext durante a geração do bundle.
# Não é levada para a imagem final.
ENV ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=build;Username=build;Password=build"

WORKDIR /workspace

RUN dotnet tool run dotnet-ef migrations bundle \
    --project "src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj" \
    --startup-project "src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj" \
    --context "ApplicationDbContext" \
    --configuration "$BUILD_CONFIGURATION" \
    --self-contained \
    --target-runtime linux-x64 \
    --output "/app/migrations/efbundle" \
    --force

# Imagem final de produção.
FROM base AS final

ARG BUILD_REVISION=local
ARG BUILD_VERSION=0.0.0-local

LABEL org.opencontainers.image.title="Corporate Service Desk API" \
      org.opencontainers.image.description="API REST para gerenciamento de chamados corporativos." \
      org.opencontainers.image.source="https://github.com/Peterson-Benhame/CorporateServiceDesk" \
      org.opencontainers.image.revision="${BUILD_REVISION}" \
      org.opencontainers.image.version="${BUILD_VERSION}"

ENV APP_VERSION="${BUILD_REVISION}"

WORKDIR /app

COPY --from=publish /app/publish .
COPY --chmod=755 --from=migrations /app/migrations/efbundle ./efbundle
COPY --chmod=755 src/CorporateServiceDesk.Api/docker-entrypoint.sh ./docker-entrypoint.sh

ENTRYPOINT ["./docker-entrypoint.sh"]
