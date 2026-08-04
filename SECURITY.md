# Política de Segurança

## Relato de vulnerabilidade

Não publique credenciais, detalhes exploráveis ou dados sensíveis em issues públicas.

Para um problema de segurança, entre em contato diretamente com o mantenedor pelo perfil do GitHub do projeto.

## Controles do repositório

O projeto utiliza:

- CodeQL para análise estática de C#;
- Dependency Review em Pull Requests;
- Trivy para vulnerabilidades, secrets e configurações inseguras;
- Dependabot para NuGet, GitHub Actions e imagens Docker;
- GitHub Secret Scanning e Push Protection;
- GitHub Environment protegido para o deploy de produção;
- credenciais de produção armazenadas somente no Render.

## Credenciais

- Nunca inclua connection strings, tokens ou deploy hooks no repositório.
- O arquivo `.env` é exclusivamente local e permanece ignorado pelo Git.
- Se um secret for exposto, faça a rotação imediatamente.
- O GitHub Actions não recebe a credencial do PostgreSQL de produção.

## Dependências

Vulnerabilidades críticas corrigíveis bloqueiam a publicação da imagem Docker.

Atualizações de dependências são propostas pelo Dependabot e precisam passar por todos os checks do CI.
