# Visão do Produto

## Produto

Corporate Service Desk API

## Visão

Fornecer uma API corporativa de referência para centralizar o registro, acompanhamento e atendimento de chamados internos.

O projeto também servirá como laboratório prático e item de portfólio para demonstrar organização arquitetural, segurança, testes, documentação, containers e integração contínua.

## Problema

Solicitações internas frequentemente são registradas por e-mail, mensagens ou conversas informais.

Esse modelo dificulta:

- localizar solicitações antigas;
- acompanhar o andamento;
- atribuir responsáveis;
- controlar acesso;
- registrar decisões;
- gerar histórico;
- investigar falhas no processo.

## Solução proposta

Criar uma API REST que permita:

- autenticar usuários;
- abrir chamados;
- consultar chamados;
- atribuir atendentes;
- alterar prioridade e status;
- adicionar comentários;
- gerenciar perfis e permissões;
- registrar alterações relevantes.

## Usuários

### Solicitante

Funcionário que registra e acompanha chamados.

### Atendente

Profissional responsável por tratar e resolver chamados.

### Administrador

Responsável por usuários, perfis, permissões e configurações administrativas.

## Objetivo do MVP

Entregar um fluxo funcional que permita:

1. autenticar um usuário;
2. abrir um chamado;
3. consultar chamados;
4. atribuir um atendente;
5. alterar o status;
6. registrar comentários;
7. controlar o acesso por permissões.

## Diferenciais técnicos

- .NET 8;
- ASP.NET Core Web API;
- arquitetura em camadas;
- Entity Framework Core;
- JWT;
- autorização baseada em permissões;
- paginação e filtros;
- logs estruturados;
- tratamento padronizado de erros;
- testes automatizados;
- Docker;
- pipeline.

## Fora do escopo inicial

- anexos;
- envio de e-mail;
- notificações em tempo real;
- SLA;
- dashboards;
- relatórios avançados;
- múltiplas empresas;
- Active Directory;
- refresh token;
- mensageria;
- microsserviços.

## Critério de sucesso do projeto

O projeto será considerado concluído quando:

- os fluxos principais funcionarem;
- autenticação e autorização estiverem aplicadas;
- consultas possuírem paginação;
- regras relevantes estiverem testadas;
- a aplicação executar em Docker;
- a pipeline compilar e executar os testes;
- a documentação permitir que outra pessoa execute e compreenda o projeto.