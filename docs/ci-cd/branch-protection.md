# Proteção da branch `master`

A proteção é configurada no GitHub e não pode ser aplicada apenas por arquivos do repositório.

## Configuração recomendada

Acesse:

```text
Settings → Rules → Rulesets → New branch ruleset
```

Defina:

- nome: `Protect master`;
- enforcement: `Active`;
- target branch: `master`;
- bloquear exclusão da branch;
- bloquear force push;
- exigir Pull Request antes do merge;
- exigir resolução das conversas;
- exigir histórico linear;
- exigir que a branch esteja atualizada antes do merge;
- exigir status checks.

## Status checks obrigatórios

Depois que os novos workflows executarem ao menos uma vez, selecione:

- `Build, Tests, Coverage, Container and Security`;
- `CodeQL`;
- `Dependency Review`;
- `Repository Security Scan`.

Para um repositório pessoal, não exija aprovação de outra pessoa no Pull Request. O controle principal será o PR obrigatório e os status checks.

## Ambiente de produção

Mantenha o Environment `production` com:

- branch permitida: `master`;
- Required reviewer: `Peterson-Benhame`;
- `Prevent self-review`: desativado enquanto você for o único aprovador;
- secret: `RENDER_DEPLOY_HOOK_URL`.

Remova o secret `PRODUCTION_DATABASE_CONNECTION` do GitHub. As migrations de produção são executadas dentro da imagem no Render.
