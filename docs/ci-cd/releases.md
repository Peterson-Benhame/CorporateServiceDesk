# Versionamento e releases

O projeto utiliza Release Please e Conventional Commits.

## Funcionamento

Cada push na `master` atualiza ou cria um Pull Request de release.

Quando o PR de release é mesclado:

- o `CHANGELOG.md` é atualizado;
- uma tag SemVer é criada;
- uma GitHub Release é publicada;
- a imagem Docker recebe tags:
  - `v1.2.3`;
  - `1.2.3`;
  - `1.2`;
  - `1`;
  - `latest`.

## Token recomendado

Crie um Fine-grained Personal Access Token e salve como:

```text
RELEASE_PLEASE_TOKEN
```

Permissões mínimas no repositório:

- Contents: Read and write;
- Pull requests: Read and write;
- Issues: Read and write.

O workflow possui fallback para `GITHUB_TOKEN`, porém eventos criados pelo token padrão podem não disparar outros workflows. O token dedicado permite que o CI execute normalmente nos Pull Requests de release.

## Commits

- `fix:` gera versão patch;
- `feat:` gera versão minor;
- `feat!:` ou `BREAKING CHANGE:` gera versão major.
