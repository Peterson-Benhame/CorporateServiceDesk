## Objetivo

Descreva de forma direta o problema resolvido e o resultado esperado.

## Alterações

- [ ] Código de aplicação
- [ ] Testes
- [ ] Migration
- [ ] Docker
- [ ] CI/CD
- [ ] Documentação

## Validações

- [ ] `dotnet restore CorporateServiceDesk.sln --locked-mode`
- [ ] `dotnet build CorporateServiceDesk.sln --configuration Release --no-restore`
- [ ] `dotnet test CorporateServiceDesk.sln --configuration Release --no-build`
- [ ] O modelo do EF não possui alteração sem migration
- [ ] A imagem Docker inicia e responde em `/health`
- [ ] Nenhum secret foi incluído no código ou nos logs

## Migration

- [ ] Não se aplica
- [ ] Criada e testada no PostgreSQL local
- [ ] Revisada para evitar perda inesperada de dados

## Evidências

Inclua logs, capturas ou exemplos de requisição quando forem úteis.
