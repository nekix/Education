#!/bin/bash
set -e

echo "Запуск миграций EF Core..."

echo "Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Timezone=UTC"

cd /app/CarPark.Infrastructure

# Применить миграции (EF Core соберет проект автоматически)
dotnet ef database update \
    --project /app/CarPark.Infrastructure/CarPark.Infrastructure.csproj \
    --verbose \
    -- "Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Timezone=UTC"


# Проверка успешности миграций
if [ $? -eq 0 ]; then
    echo "Миграции применены успешно!"
else
    echo "Ошибка при применении миграций!"
    exit 1
fi