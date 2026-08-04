# Imagem base da aplicação.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /workspace

COPY ["Directory.Packages.props", "./"]

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

WORKDIR /app

COPY --from=publish /app/publish .
COPY --chmod=755 --from=migrations /app/migrations/efbundle ./efbundle
COPY --chmod=755 src/CorporateServiceDesk.Api/docker-entrypoint.sh ./docker-entrypoint.sh

ENTRYPOINT ["./docker-entrypoint.sh"]