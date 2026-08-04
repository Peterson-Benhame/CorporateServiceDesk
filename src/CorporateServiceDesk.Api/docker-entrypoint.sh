#!/bin/sh
set -e

if [ -z "${ConnectionStrings__DefaultConnection:-}" ]; then
  echo "A variável ConnectionStrings__DefaultConnection não foi configurada."
  exit 1
fi

echo "Verificando migrations pendentes..."

./efbundle \
  --connection "$ConnectionStrings__DefaultConnection" \
  --no-color

echo "Banco de dados atualizado."

exec dotnet CorporateServiceDesk.Api.dll