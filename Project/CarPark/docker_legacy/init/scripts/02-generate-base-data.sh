#!/bin/bash
set -e

echo "Генерация базовых данных..."

cd /app

# Проверка, что базовые данные еще не сгенерированы
export PGPASSWORD=$POSTGRES_PASSWORD
EXISTING_MODELS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM models;" 2>/dev/null || echo "0")
EXISTING_TZINFO=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM tz_info;" 2>/dev/null || echo "0")
EXISTING_ENTERPRISES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM enterprises;" 2>/dev/null || echo "0")
EXISTING_VEHICLES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM vehicles;" 2>/dev/null || echo "0")
EXISTING_DRIVERS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM drivers;" 2>/dev/null || echo "0")

if [ "$EXISTING_ENTERPRISES" -gt 0 ] || [ "$EXISTING_VEHICLES" -gt 0 ] || [ "$EXISTING_DRIVERS" -gt 0 ]; then
    echo "Базовые данные уже существуют:"
    echo "  Предприятия: $EXISTING_ENTERPRISES"
    echo "  Автомобили: $EXISTING_VEHICLES"
    echo "  Водители: $EXISTING_DRIVERS"
    echo "Пропускаем генерацию базовых данных..."
    exit 0
fi

# 1. Генерация справочников
if [ "$EXISTING_MODELS" -eq 0 ] || [ "$EXISTING_TZINFO" -eq 0 ]; then
    echo "1 Генерация справочников (Models, TzInfo)..."
    dotnet /app/CarPark.DataGenerator/CarPark.DataGenerator.dll \
        generate seed-reference \
        --seed ${DEMO_SEED:-42} \
        --connection-string "$CONNECTION_STRING"

    if [ $? -ne 0 ]; then
        echo "Ошибка при генерации справочников!"
        exit 1
    fi
else
    echo "1 Справочники уже существуют (Models: $EXISTING_MODELS, TzInfo: $EXISTING_TZINFO)"
fi

echo ""

# 2. Генерация полного набора данных
echo "2 Генерация предприятий, автомобилей, водителей..."
dotnet /app/CarPark.DataGenerator/CarPark.DataGenerator.dll \
    generate full-demo \
    --seed ${DEMO_SEED:-42} \
    --enterprises ${DEMO_ENTERPRISES:-3} \
    --vehicles-per-enterprise ${DEMO_VEHICLES:-30} \
    --drivers-per-enterprise ${DEMO_DRIVERS:-50} \
    --export-vehicle-ids /tmp/vehicles.txt \
    --connection-string "$CONNECTION_STRING"

if [ $? -ne 0 ]; then
    echo "Ошибка при генерации базовых данных!"
    exit 1
fi

echo ""

echo "Базовые данные сгенерированы!"

# Проверка наличия файла с vehicle IDs
if [ ! -f /tmp/vehicles.txt ]; then
    echo "Файл /tmp/vehicles.txt не создан!"
    exit 1
fi

VEHICLE_COUNT=$(wc -l < /tmp/vehicles.txt)
echo "📋 Экспортировано $VEHICLE_COUNT активных автомобилей"