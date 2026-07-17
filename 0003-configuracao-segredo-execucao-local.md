### Configurar o segredo para execução local

Não coloque a senha em:

```text
appsettings.json
appsettings.Development.json
launchSettings.json
```

Use o Secret Manager do ASP.NET Core. Ele mantém os segredos fora da pasta do projeto e reduz o risco de commit acidental. ([Microsoft Learn][2])

Abra um PowerShell na raiz da solution.

Execute:

```powershell
dotnet user-secrets init --project src/CorporateServiceDesk.Api
```

Esse comando adiciona um `UserSecretsId` ao `.csproj` da API.

Agora registre a conexão:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=corporate_service_desk;Username=corporate_service_desk;Password=SUA_SENHA_LOCAL" --project src/CorporateServiceDesk.Api
```

Substitua:

```text
SUA_SENHA_LOCAL
```

por uma senha local criada por você.

Confira se o segredo foi registrado:

```powershell
dotnet user-secrets list --project src/CorporateServiceDesk.Api
```

A senha poderá aparecer nesse comando. Não envie esse resultado em prints ou commits.

---

# 4. Criar o `.env`

Na raiz da solution, ao lado do `.sln`, crie:

```text
.env
```

Conteúdo:

```env
POSTGRES_DB=corporate_service_desk
POSTGRES_USER=corporate_service_desk
POSTGRES_PASSWORD=SUA_SENHA_LOCAL
```

Use a mesma senha configurada no Secret Manager.

## Proteja o arquivo

Abra o `.gitignore` da raiz e acrescente:

```gitignore
.env
```

Crie também:

```text
.env.example
```

Conteúdo:

```env
POSTGRES_DB=corporate_service_desk
POSTGRES_USER=corporate_service_desk
POSTGRES_PASSWORD=replace_with_local_password
```

O `.env.example` pode ser versionado. O `.env` real não.

O Docker Compose consegue interpolar variáveis de um arquivo `.env`, e a imagem oficial do PostgreSQL utiliza `POSTGRES_DB`, `POSTGRES_USER` e `POSTGRES_PASSWORD` para sua configuração inicial. ([Docker Documentation][3])

---

# 5. Criar `compose.yaml`

Na raiz da solution, crie:

```text
compose.yaml
```

Conteúdo:

```yaml
services:
  postgres:
    image: postgres:16-alpine
    container_name: corporate-service-desk-postgres

    environment:
      POSTGRES_DB: ${POSTGRES_DB}
      POSTGRES_USER: ${POSTGRES_USER}
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD}

    ports:
      - "5432:5432"

    volumes:
      - corporate-service-desk-postgres-data:/var/lib/postgresql/data

    healthcheck:
      test:
        [
          "CMD-SHELL",
          "pg_isready -U ${POSTGRES_USER} -d ${POSTGRES_DB}"
        ]
      interval: 5s
      timeout: 5s
      retries: 10

  api:
    container_name: corporate-service-desk-api

    build:
      context: .
      dockerfile: src/CorporateServiceDesk.Api/Dockerfile

    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ASPNETCORE_URLS: http://+:8080
      ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}

    ports:
      - "8080:8080"

    depends_on:
      postgres:
        condition: service_healthy

volumes:
  corporate-service-desk-postgres-data:
```

### Por que temos duas connection strings?

Na execução local:

```text
Host=localhost
```

Porque a API roda no seu Windows e acessa a porta publicada pelo Docker.

No Docker Compose:

```text
Host=postgres
```

Porque `postgres` é o nome do serviço dentro da rede criada pelo Compose.

---

# 6. Revisar o Dockerfile

A API agora referencia:

```text
Application
Domain
Infrastructure
```

No estágio de build do Dockerfile, todos os `.csproj` precisam ser copiados antes do `dotnet restore`.

Localize a linha que copia o projeto da API e mantenha este conjunto:

```dockerfile
COPY ["src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj", "src/CorporateServiceDesk.Api/"]
COPY ["src/CorporateServiceDesk.Application/CorporateServiceDesk.Application.csproj", "src/CorporateServiceDesk.Application/"]
COPY ["src/CorporateServiceDesk.Domain/CorporateServiceDesk.Domain.csproj", "src/CorporateServiceDesk.Domain/"]
COPY ["src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj", "src/CorporateServiceDesk.Infrastructure/"]
```

Depois:

```dockerfile
RUN dotnet restore "./src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj"
```

Não altere o restante do Dockerfile quando esses caminhos já estiverem consistentes com sua estrutura atual.

---

# 7. Subir primeiro somente o PostgreSQL

Na raiz da solution:

```powershell
docker compose up -d postgres
```

Esse comando:

* lê o `compose.yaml`;
* lê o `.env`;
* baixa a imagem quando necessário;
* cria o volume;
* inicia o PostgreSQL.

Confira:

```powershell
docker compose ps
```

Resultado esperado:

```text
corporate-service-desk-postgres   Up   healthy
```

Também pode verificar os logs:

```powershell
docker compose logs postgres
```

Procure uma mensagem indicando que o banco está pronto para aceitar conexões.

---

# 8. Verificar a ferramenta do Entity Framework

Execute:

```powershell
dotnet ef --version
```

O ideal é que a ferramenta esteja na mesma versão principal do EF Core utilizado no projeto.

Caso o comando não exista e seus pacotes estejam na versão `8.0.11`, instale:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
```

Feche e reabra o terminal depois da instalação.

---

# 9. Criar a primeira migration

No PowerShell:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
```

Isso garante que a execução de design utilize a configuração de desenvolvimento e encontre o User Secrets.

Agora execute:

```powershell
dotnet ef migrations add InitialCreate --project src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj --startup-project src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj --context ApplicationDbContext --output-dir Persistence/Migrations
```

### Significado dos parâmetros

```text
--project
```

Projeto onde estão o `DbContext` e onde a migration será criada.

```text
--startup-project
```

Projeto executável que fornece configuração e injeção de dependência.

```text
--context
```

Indica qual `DbContext` será utilizado.

```text
--output-dir
```

Define a pasta física das migrations.

O EF Core utiliza migrations para evoluir o esquema de forma incremental, e os arquivos gerados devem ser revisados e versionados. ([Microsoft Learn][4])

Estrutura esperada:

```text
CorporateServiceDesk.Infrastructure/
└── Persistence/
    └── Migrations/
        ├── <timestamp>_InitialCreate.cs
        ├── <timestamp>_InitialCreate.Designer.cs
        └── ApplicationDbContextModelSnapshot.cs
```

---

# 10. Revisar a migration antes de aplicar

Abra:

```text
<timestamp>_InitialCreate.cs
```

Confirme que existe algo equivalente a:

```csharp
migrationBuilder.CreateTable(
    name: "tickets",
    columns: table => new
    {
        id = table.Column<Guid>(nullable: false),
        title = table.Column<string>(nullable: false),
        description = table.Column<string>(nullable: false),
        requester_id = table.Column<Guid>(nullable: false),
        priority = table.Column<int>(nullable: false),
        status = table.Column<int>(nullable: false),
        created_at = table.Column<DateTimeOffset>(
            type: "timestamp with time zone",
            nullable: false)
    });
```

Não altere a migration apenas para deixá-la visualmente diferente.

O Npgsql aceita `DateTimeOffset` em `timestamp with time zone` quando o offset é zero. Isso está coerente com a validação UTC existente no domínio. ([Npgsql][5])

---

# 11. Aplicar a migration

Execute:

```powershell
dotnet ef database update --project src/CorporateServiceDesk.Infrastructure/CorporateServiceDesk.Infrastructure.csproj --startup-project src/CorporateServiceDesk.Api/CorporateServiceDesk.Api.csproj --context ApplicationDbContext
```

O comando aplica a migration mais recente ao banco configurado. ([Microsoft Learn][6])

Resultado esperado:

```text
Applying migration '..._InitialCreate'
Done.
```

---

# 12. Confirmar as tabelas no PostgreSQL

Execute:

```powershell
docker exec -it corporate-service-desk-postgres psql -U corporate_service_desk -d corporate_service_desk -c "\dt"
```

Resultado esperado:

```text
Schema | Name                  | Type
public | __EFMigrationsHistory | table
public | tickets               | table
```

A tabela:

```text
__EFMigrationsHistory
```

é usada pelo EF Core para controlar quais migrations já foram aplicadas.

---

# 13. Subir API e PostgreSQL juntos

Execute:

```powershell
docker compose up --build
```

Depois acesse:

```text
http://localhost:8080/swagger
```

O `GET /WeatherForecast` ainda deverá responder `200 OK`.

Nesse momento, a API já possui conexão configurada, mas ainda não utiliza o banco em um endpoint.

---

## Como validar a etapa

Execute:

```powershell
dotnet build
dotnet test
docker compose ps
```

Confirme:

* build sem erros;
* testes passando;
* PostgreSQL saudável;
* migration aplicada;
* tabela `tickets` criada;
* Swagger acessível pelo Compose;
* `.env` não aparece no `git status`.

## Erro comum: senha não corresponde ao volume

O PostgreSQL utiliza as variáveis de criação apenas quando o diretório de dados está vazio.

Como este projeto ainda não possui dados importantes, caso tenha criado o volume com outras credenciais, você pode removê-lo:

```powershell
docker compose down -v
```

**Atenção:** `-v` apaga o banco local e todos os dados do volume.

Depois:

```powershell
docker compose up -d postgres
```


### ERRO

````markdown
# Falha de autenticação no PostgreSQL durante aplicação da migration

## Status

Resolvido.

## Contexto

Durante a configuração do Entity Framework Core com PostgreSQL, a migration `InitialCreate` foi gerada corretamente.

Porém, ao tentar aplicá-la com o comando:

```powershell
dotnet ef database update `
  --project .\src\CorporateServiceDesk.Infrastructure\CorporateServiceDesk.Infrastructure.csproj `
  --startup-project .\src\CorporateServiceDesk.Api\CorporateServiceDesk.Api.csproj `
  --context ApplicationDbContext
````

a conexão com o banco falhava durante a autenticação.

---

## Erro apresentado

```text
Npgsql.PostgresException: 28P01: password authentication failed
for user "corporate_service_desk"
```

O código PostgreSQL `28P01` indica que o servidor acessado recusou a senha informada para o usuário.

---

## Ambiente

O computador possuía duas instâncias de PostgreSQL:

1. PostgreSQL instalado diretamente no Windows, utilizado pelo pgAdmin;
2. PostgreSQL executado em um container Docker.

Inicialmente, ambos eram acessados pela porta padrão:

```text
5432
```

A configuração local da aplicação estava assim:

```text
Host=localhost;
Port=5432;
Database=corporate_service_desk;
Username=corporate_service_desk;
Password=***
```

O serviço PostgreSQL do Docker também publicava:

```yaml
ports:
  - "5432:5432"
```

---

## Hipóteses investigadas

### 1. Senha incorreta no User Secrets

A connection string foi atualizada no Secret Manager, mas o erro continuou.

### 2. Volume Docker criado com uma senha antiga

O container e o volume foram removidos:

```powershell
docker compose down -v
```

Depois, o PostgreSQL foi recriado:

```powershell
docker compose up -d postgres
```

Mesmo após a recriação, o erro continuou.

### 3. PostgreSQL do container com configuração inválida

A autenticação foi testada diretamente dentro do container:

```powershell
docker exec corporate-service-desk-postgres sh -lc `
  'PGPASSWORD="$POSTGRES_PASSWORD" psql -h 127.0.0.1 -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c "select current_user, current_database();"'
```

O comando funcionou.

Isso comprovou que:

* o container estava saudável;
* o usuário existia;
* o banco existia;
* a senha configurada no container estava correta;
* o PostgreSQL do container aceitava as credenciais.

---

## Causa raiz

A aplicação estava utilizando:

```text
localhost:5432
```

Porém, o computador também possuía uma instalação local do PostgreSQL usando a porta padrão.

Com isso, o Entity Framework estava alcançando a instância PostgreSQL instalada no Windows, e não a instância executada pelo Docker.

A instância local possuía credenciais diferentes e retornava:

```text
28P01: password authentication failed
```

Portanto, o problema não estava:

* na migration;
* no `ApplicationDbContext`;
* no Entity Framework Core;
* no provedor Npgsql;
* no usuário criado pelo container;
* na senha armazenada pelo PostgreSQL do Docker.

O problema estava no endpoint utilizado para acessar o banco.

---

## Resolução

A porta publicada pelo container PostgreSQL foi alterada para uma porta exclusiva no computador.

### Antes

```yaml
ports:
  - "5432:5432"
```

### Depois

```yaml
ports:
  - "55432:5432"
```

A leitura dessa configuração é:

```text
Porta 55432 do Windows → porta 5432 do container
```

O PostgreSQL continua escutando na porta `5432` dentro do container.

---

## Recriação do container

Depois da alteração no `compose.yaml`, o container foi recriado:

```powershell
docker compose down
docker compose up -d postgres
```

A publicação da porta foi validada com:

```powershell
docker compose ps
```

Resultado:

```text
0.0.0.0:55432->5432/tcp
```

---

## Connection string para execução local

A connection string utilizada por comandos executados no Windows passou a usar:

```text
Host=127.0.0.1;
Port=55432;
Database=corporate_service_desk;
Username=corporate_service_desk;
Password=***
```

Ela foi armazenada no User Secrets:

```powershell
dotnet user-secrets set `
  "ConnectionStrings:DefaultConnection" `
  "Host=127.0.0.1;Port=55432;Database=corporate_service_desk;Username=corporate_service_desk;Password=SENHA_LOCAL" `
  --project .\src\CorporateServiceDesk.Api\CorporateServiceDesk.Api.csproj
```

A senha real não deve ser adicionada ao repositório.

---

## Connection string dentro do Docker Compose

A API executada dentro do Docker não utiliza a porta publicada no Windows.

Dentro da rede do Compose, a API acessa o banco pelo nome do serviço:

```text
Host=postgres;
Port=5432;
```

Exemplo:

```yaml
environment:
  ConnectionStrings__DefaultConnection: Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
```

A separação ficou assim:

| Origem da conexão                | Host        |   Porta |
| -------------------------------- | ----------- | ------: |
| Aplicação executada no Windows   | `127.0.0.1` | `55432` |
| `dotnet ef` executado no Windows | `127.0.0.1` | `55432` |
| pgAdmin acessando o container    | `127.0.0.1` | `55432` |
| API executada no Docker Compose  | `postgres`  |  `5432` |
| PostgreSQL instalado no Windows  | `localhost` |  `5432` |

---

## Aplicação da migration

Após alterar a porta, a migration foi aplicada:

```powershell
dotnet ef database update `
  --project .\src\CorporateServiceDesk.Infrastructure\CorporateServiceDesk.Infrastructure.csproj `
  --startup-project .\src\CorporateServiceDesk.Api\CorporateServiceDesk.Api.csproj `
  --context ApplicationDbContext
```

Resultado:

```text
Applying migration '20260717203556_InitialCreate'.
Done.
```

---

## Estruturas criadas

O Entity Framework criou a tabela de controle de migrations:

```text
__EFMigrationsHistory
```

Também criou a tabela:

```text
tickets
```

Com as colunas:

```text
id
title
description
requester_id
priority
status
created_at
```

---

## Validação

As tabelas podem ser verificadas com:

```powershell
docker exec -it corporate-service-desk-postgres `
  psql `
  -U corporate_service_desk `
  -d corporate_service_desk `
  -c "\dt"
```

Resultado esperado:

```text
public | __EFMigrationsHistory | table
public | tickets               | table
```

---

## Aprendizado

Uma falha de autenticação não significa necessariamente que a senha configurada está incorreta.

Primeiro é necessário confirmar:

1. qual servidor recebeu a conexão;
2. qual host e porta foram utilizados;
3. se existem outras instâncias do banco no ambiente;
4. se a aplicação local e os containers utilizam endpoints diferentes.

O processo de investigação foi:

```text
Falha de autenticação
        ↓
Verificação das credenciais
        ↓
Recriação do volume
        ↓
Teste direto dentro do container
        ↓
Confirmação de que o container aceitava a senha
        ↓
Identificação de outra instância PostgreSQL local
        ↓
Alteração da porta publicada
        ↓
Migration aplicada com sucesso
```

---

## Prevenção

Para evitar o mesmo problema:

* não publicar dois serviços na mesma porta lógica de acesso;
* utilizar uma porta exclusiva para o PostgreSQL do Docker;
* documentar a diferença entre conexão local e conexão entre containers;
* verificar o endpoint antes de alterar credenciais;
* não versionar `.env` com valores reais;
* armazenar a connection string local no User Secrets;
* não registrar senhas em logs, prints ou documentação;
* usar `.env.example` sem credenciais reais;
* trocar imediatamente qualquer senha exposta durante a investigação.

---

## Configuração final

### PostgreSQL instalado no Windows

```text
Host: localhost
Porta: 5432
```

### PostgreSQL do Docker

```text
Host: 127.0.0.1
Porta externa: 55432
Porta interna: 5432
```

### Comunicação entre API e PostgreSQL dentro do Docker

```text
Host: postgres
Porta: 5432
```

```
```
