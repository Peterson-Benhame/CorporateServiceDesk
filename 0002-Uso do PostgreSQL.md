# ADR-0002 — Uso do PostgreSQL

## Status

Aceito.

## Decisão

O projeto utilizará PostgreSQL como sistema gerenciador de banco de dados.

O acesso ao banco será realizado por meio do Entity Framework Core e do provedor Npgsql.

## Consequências

- a Infrastructure dependerá do provedor `Npgsql.EntityFrameworkCore.PostgreSQL`;
- as migrations serão geradas para PostgreSQL;
- o ambiente local utilizará PostgreSQL em container;
- configurações e consultas devem respeitar os comportamentos do PostgreSQL;
- o banco poderá ser executado junto com a API por Docker Compose.