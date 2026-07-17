# Requisitos do MVP

## 1. Requisitos funcionais

### RF-001 — Autenticar usuário

O sistema deve autenticar usuários por credenciais válidas e emitir um JWT.

### RF-002 — Bloquear usuário inativo

Usuários inativos não devem conseguir autenticar-se.

### RF-003 — Criar chamado

Um usuário autorizado deve conseguir abrir um chamado informando:

- título;
- descrição;
- prioridade.

O solicitante deve ser identificado pelo usuário autenticado.

### RF-004 — Consultar chamado

Um usuário autorizado deve conseguir consultar um chamado por identificador.

O acesso deve respeitar as permissões e a relação do usuário com o chamado.

### RF-005 — Listar chamados

O sistema deve permitir listar chamados com:

- paginação;
- ordenação;
- filtro por status;
- filtro por prioridade;
- filtro por solicitante;
- filtro por atendente;
- filtro por período;
- pesquisa por título.

### RF-006 — Atribuir atendente

Um usuário autorizado deve conseguir atribuir um atendente ao chamado.

### RF-007 — Alterar status

Um atendente ou administrador autorizado deve conseguir alterar o status do chamado respeitando as transições válidas.

### RF-008 — Adicionar comentário

Usuários autorizados devem conseguir adicionar comentários aos chamados aos quais possuem acesso.

### RF-009 — Gerenciar usuários

Um administrador deve conseguir cadastrar, consultar, ativar e desativar usuários.

### RF-010 — Gerenciar perfis e permissões

Um administrador deve conseguir:

- cadastrar perfis;
- associar permissões aos perfis;
- associar perfis aos usuários.

## 2. Regras de negócio iniciais

### RN-001

Todo chamado deve possuir um solicitante.

### RN-002

Todo chamado novo deve iniciar com status `Open`.

### RN-003

Um chamado deve possuir título e descrição.

### RN-004

Somente usuários autorizados podem visualizar chamados de terceiros.

### RN-005

Somente atendentes ou administradores autorizados podem assumir chamados.

### RN-006

Um chamado precisa estar resolvido antes de ser encerrado.

### RN-007

Usuários inativos não podem autenticar-se.

### RN-008

Alterações relevantes devem registrar data e usuário responsável.

## 3. Requisitos não funcionais

### RNF-001 — Tecnologia

A aplicação deve ser desenvolvida em .NET 8 com ASP.NET Core.

### RNF-002 — Persistência

A persistência deve utilizar Entity Framework Core.

O banco de dados será PostgreSQL, acessado por meio do Entity Framework Core e do provedor Npgsql.

### RNF-003 — Segurança

- senhas não podem ser armazenadas em texto puro;
- segredos não podem ser versionados;
- tokens e senhas não podem aparecer nos logs;
- autorização deve ser validada no servidor;
- erros não podem expor stack trace ao cliente.

### RNF-004 — Consultas

Coleções devem possuir paginação e limite máximo por página.

### RNF-005 — Erros

A API deve utilizar respostas padronizadas com Problem Details.

### RNF-006 — Logs

A aplicação deve produzir logs estruturados e correlacionáveis.

### RNF-007 — Testes

O sistema deve possuir:

- testes unitários para regras e casos de uso;
- testes de integração para endpoints e persistência.

### RNF-008 — Containers

A aplicação deve executar em container Linux.

### RNF-009 — Pipeline

A pipeline deve executar no mínimo:

1. restore;
2. build;
3. testes;
4. geração do artefato ou imagem.

## 4. Restrições

- não utilizar microsserviços no MVP;
- não adicionar mensageria sem requisito;
- não expor entidades diretamente como contrato HTTP;
- não retornar coleções ilimitadas;
- não expor `IQueryable` fora da Infrastructure;
- não implementar regras de negócio nos controllers.

## 5. Pontos pendentes

- definição entre SQL Server e PostgreSQL;
- definição entre GitHub Actions e Jenkins;
- limites máximos de título e descrição;
- política de expiração do JWT;
- regras completas de reabertura de chamados;
- critérios para acesso de atendentes aos chamados.