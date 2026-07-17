# User Stories do MVP

## US-001 — Autenticar usuário

Como usuário cadastrado,  
quero autenticar-me,  
para acessar as funcionalidades permitidas.

### Critérios de aceitação

- credenciais válidas retornam um JWT;
- credenciais inválidas retornam resposta genérica;
- usuário inativo não consegue autenticar-se;
- o token contém a identificação necessária;
- o token não contém senha ou dado sensível.

---

## US-002 — Abrir chamado

Como solicitante,  
quero abrir um chamado,  
para registrar uma necessidade de atendimento.

### Critérios de aceitação

- título é obrigatório;
- descrição é obrigatória;
- prioridade deve ser válida;
- solicitante é obtido pelo usuário autenticado;
- chamado inicia como `Open`;
- data de criação é gerada pelo servidor;
- resposta retorna o identificador criado.

---

## US-003 — Consultar meus chamados

Como solicitante,  
quero consultar meus chamados,  
para acompanhar o atendimento.

### Critérios de aceitação

- usuário visualiza apenas os próprios chamados;
- listagem possui paginação;
- filtros inválidos são rejeitados;
- ordenação possui comportamento previsível;
- chamado inexistente retorna resposta adequada.

---

## US-004 — Consultar chamados para atendimento

Como atendente,  
quero consultar chamados disponíveis,  
para identificar solicitações que precisam de atendimento.

### Critérios de aceitação

- usuário precisa possuir `tickets.read.all`;
- consulta possui paginação;
- pode filtrar por status e prioridade;
- resultados respeitam o escopo de acesso do atendente.

---

## US-005 — Atribuir chamado

Como atendente,  
quero assumir ou atribuir um chamado,  
para registrar quem será responsável pelo atendimento.

### Critérios de aceitação

- usuário precisa possuir `tickets.assign`;
- chamado deve existir;
- atendente informado deve estar ativo;
- alteração deve registrar data e responsável;
- conflitos de atribuição devem ser tratados.

---

## US-006 — Alterar status do chamado

Como atendente,  
quero alterar o status de um chamado,  
para registrar seu andamento.

### Critérios de aceitação

- usuário precisa possuir `tickets.change-status`;
- somente transições permitidas são aceitas;
- chamado resolvido pode ser encerrado;
- chamado não resolvido não pode ser encerrado;
- alteração registra data e responsável.

---

## US-007 — Adicionar comentário

Como usuário autorizado,  
quero adicionar um comentário,  
para registrar informações sobre o chamado.

### Critérios de aceitação

- comentário não pode ser vazio;
- usuário deve possuir acesso ao chamado;
- comentário registra autor e data;
- o comentário não pode ser alterado silenciosamente.

---

## US-008 — Gerenciar usuários e acessos

Como administrador,  
quero gerenciar usuários, perfis e permissões,  
para controlar o acesso ao sistema.

### Critérios de aceitação

- somente usuários autorizados acessam a funcionalidade;
- usuário pode ser ativado ou desativado;
- perfis agrupam permissões;
- permissões duplicadas não são associadas;
- alterações administrativas devem ser rastreáveis.