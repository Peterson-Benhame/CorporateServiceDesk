# Contribuindo

## Fluxo de branches

Crie uma branch a partir de `master`:

```powershell
git switch master
git pull
git switch -c feat/nome-da-funcionalidade
```

Prefixos recomendados:

- `feat/`: funcionalidade;
- `fix/`: correção;
- `refactor/`: refatoração;
- `test/`: testes;
- `docs/`: documentação;
- `ci/`: CI/CD.

Não faça push direto na `master`.

## Conventional Commits

O projeto usa Conventional Commits para gerar versões e changelog.

Exemplos:

```text
feat: add ticket assignment endpoint
fix: prevent duplicate active tickets
test: cover ticket search validation
ci: validate production container startup
docs: document migration workflow
```

Alteração incompatível:

```text
feat!: change ticket creation contract
```

## Migrations locais

O desenvolvedor cria e testa migrations somente no PostgreSQL local. A credencial de produção não é disponibilizada ao desenvolvedor nem ao GitHub Actions.

Criar migration:

```powershell
docker compose -f docker-compose.yml --profile tools run --rm migrations "dotnet tool restore && dotnet ef migrations add NomeDaMigration --project src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj --startup-project src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj --context ApplicationDbContext"
```

Aplicar localmente:

```powershell
docker compose -f docker-compose.yml --profile tools run --rm migrations "dotnet tool restore && dotnet ef database update --project src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj --startup-project src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj --context ApplicationDbContext"
```

Remover a última migration ainda não publicada:

```powershell
docker compose -f docker-compose.yml --profile tools run --rm migrations "dotnet tool restore && dotnet ef migrations remove --project src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj --startup-project src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj --context ApplicationDbContext"
```

O CI executa `has-pending-model-changes` e bloqueia alterações de modelo sem migration.

## Pull Request

Antes de abrir o PR:

```powershell
dotnet tool restore
dotnet restore CorporateServiceDesk.sln --locked-mode
dotnet build CorporateServiceDesk.sln --configuration Release --no-restore
dotnet test CorporateServiceDesk.sln --configuration Release --no-build
```

O merge só deve ocorrer depois que os checks obrigatórios estiverem verdes.
