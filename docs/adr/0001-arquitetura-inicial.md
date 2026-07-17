# ADR-0001 — Arquitetura Inicial

## Status

Aceito.

## Decisão

O projeto utilizará uma **arquitetura em camadas inspirada nos princípios da Clean Architecture**.

A solution será organizada nos seguintes projetos:

```text
CorporateServiceDesk.Api
CorporateServiceDesk.Application
CorporateServiceDesk.Domain
CorporateServiceDesk.Infrastructure
```

### CorporateServiceDesk.Domain

Responsável pelas regras centrais do negócio:

* entidades;
* objetos de valor;
* enums;
* invariantes;
* exceções de domínio;
* comportamentos das entidades.

Não deve depender das demais camadas nem de frameworks externos.

### CorporateServiceDesk.Application

Responsável pelos casos de uso da aplicação:

* comandos e consultas;
* orquestração dos fluxos;
* contratos de entrada e saída;
* interfaces para dependências externas.

Depende apenas do `Domain`.

### CorporateServiceDesk.Infrastructure

Responsável pelos detalhes técnicos:

* Entity Framework Core;
* banco de dados;
* implementações de persistência;
* geração de JWT;
* hash de senhas;
* integrações externas.

Depende de `Application` e `Domain`.

### CorporateServiceDesk.Api

Responsável pela entrada HTTP e pela composição da aplicação:

* controllers;
* contratos HTTP;
* autenticação;
* autorização;
* Swagger;
* middlewares;
* configuração da injeção de dependência.

Depende de `Application` e `Infrastructure`.

## Direção das dependências

```text
Api → Application
Api → Infrastructure

Infrastructure → Application
Infrastructure → Domain

Application → Domain

Domain → nenhuma camada do sistema
```

As dependências apontam das camadas externas para as internas. O domínio não conhece ASP.NET Core, Entity Framework Core, banco de dados, JWT ou outros detalhes de infraestrutura.
