# Deploy de produção no Render

## Responsabilidades

O GitHub Actions:

1. compila e testa;
2. valida cobertura;
3. verifica migrations esquecidas;
4. inicia a imagem final com PostgreSQL temporário;
5. confirma que o migration bundle funciona;
6. verifica `/health` e `/version`;
7. analisa a imagem;
8. publica no GHCR;
9. solicita ao Render o deploy do commit exato.

O Render:

1. constrói a imagem a partir do Dockerfile;
2. injeta a connection string de produção;
3. inicia o container;
4. executa `efbundle`;
5. aplica somente migrations pendentes;
6. inicia a API apenas depois da migration;
7. disponibiliza `RENDER_GIT_COMMIT` em build e runtime.

## Configuração do serviço

No Render:

```text
Auto-Deploy: Off
Dockerfile Path: src/CorporateServiceDesk.Api/Dockerfile
Health Check Path: /health
```

Variáveis do serviço:

```text
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:10000
ConnectionStrings__DefaultConnection=<connection string interna do PostgreSQL>
```

Use a conexão interna porque API e PostgreSQL estão no Render.

## GitHub Environment

No Environment `production`, mantenha somente:

```text
RENDER_DEPLOY_HOOK_URL
```

Não armazene a connection string do banco de produção no GitHub.

## Migrations

O `docker-entrypoint.sh` executa:

```text
./efbundle --connection "$ConnectionStrings__DefaultConnection"
```

O Entity Framework consulta `__EFMigrationsHistory`:

- sem migration pendente: nenhuma alteração é realizada;
- com migration pendente: somente as ausentes são aplicadas;
- com erro: o container encerra e o deploy falha.

Esse desenho é adequado enquanto o serviço possui uma única instância. Antes de escalar para múltiplas réplicas, mova a execução para um Pre-Deploy Command ou job exclusivo para evitar concorrência entre instâncias.

## Validação do commit implantado

O deploy hook recebe:

```text
ref=<SHA do commit>
```

O endpoint `/version` usa `RENDER_GIT_COMMIT`. O pipeline só considera o deploy concluído quando:

- `/version` retorna o SHA esperado;
- `/health` retorna `Healthy`;
- `/health` retorna o mesmo SHA.

Isso evita um falso resultado positivo causado por uma versão anterior ainda saudável.
