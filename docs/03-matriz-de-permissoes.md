# Matriz de Permissões

## Perfis iniciais

- Solicitante;
- Atendente;
- Administrador.

## Permissões técnicas propostas

| Permissão | Descrição |
|---|---|
| `tickets.create` | Criar chamados |
| `tickets.read.own` | Consultar os próprios chamados |
| `tickets.read.all` | Consultar todos os chamados autorizados |
| `tickets.comment` | Adicionar comentários |
| `tickets.assign` | Atribuir atendente |
| `tickets.change-priority` | Alterar prioridade |
| `tickets.change-status` | Alterar status |
| `users.read` | Consultar usuários |
| `users.manage` | Criar, ativar e desativar usuários |
| `roles.read` | Consultar perfis |
| `roles.manage` | Gerenciar perfis e permissões |

## Matriz inicial

| Permissão | Solicitante | Atendente | Administrador |
|---|---:|---:|---:|
| `tickets.create` | Sim | Sim | Sim |
| `tickets.read.own` | Sim | Sim | Sim |
| `tickets.read.all` | Não | Sim | Sim |
| `tickets.comment` | Sim, quando possui acesso | Sim, quando possui acesso | Sim |
| `tickets.assign` | Não | Sim | Sim |
| `tickets.change-priority` | Não | Sim | Sim |
| `tickets.change-status` | Não | Sim | Sim |
| `users.read` | Não | Não | Sim |
| `users.manage` | Não | Não | Sim |
| `roles.read` | Não | Não | Sim |
| `roles.manage` | Não | Não | Sim |

## Regras adicionais

A presença de uma permissão não elimina a validação do recurso.

Exemplo:

Um solicitante com `tickets.read.own` pode consultar apenas chamados dos quais seja o solicitante.

A autorização deverá considerar:

1. identidade;
2. permissão;
3. relação com o recurso;
4. estado atual do recurso.

## Pontos pendentes

- definir se atendentes enxergam todos os chamados ou apenas os da própria fila;
- definir se comentários internos serão visíveis ao solicitante;
- definir se administradores podem alterar qualquer chamado;
- definir se um atendente pode atribuir um chamado a outro atendente.