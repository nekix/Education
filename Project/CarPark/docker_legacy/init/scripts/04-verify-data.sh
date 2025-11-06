#!/bin/bash
set -e

echo "🔍 Проверка сгенерированных данных..."

export PGPASSWORD=$POSTGRES_PASSWORD

# Проверка, что данные уже сгенерированы
EXISTING_ENTERPRISES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM enterprises;" 2>/dev/null || echo "0")
EXISTING_VEHICLES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM vehicles;" 2>/dev/null || echo "0")
EXISTING_DRIVERS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM drivers;" 2>/dev/null || echo "0")

if [ "$EXISTING_ENTERPRISES" -eq 0 ] && [ "$EXISTING_VEHICLES" -eq 0 ] && [ "$EXISTING_DRIVERS" -eq 0 ]; then
    echo "Данные еще не сгенерированы!"
    echo "Запустите сначала генерацию базовых данных."
    exit 1
fi

echo ""
echo "=== СТАТИСТИКА ДАННЫХ ==="

# Запросы к БД
ENTERPRISES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM enterprises;")
VEHICLES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM vehicles;")
DRIVERS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM drivers;")
MODELS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM models;")
GEO_POINTS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM vehicle_geo_time_points;")
RIDES=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM rides;")
MANAGERS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM managers;")

echo "Предприятия:     ${ENTERPRISES}"
echo "Модели:          ${MODELS}"
echo "Автомобили:      ${VEHICLES}"
echo "Водители:        ${DRIVERS}"
echo "Менеджеры:       ${MANAGERS}"
echo "GPS точки:       ${GEO_POINTS}"
echo "Поездки (Rides): ${RIDES}"
echo ""

# Проверки
if [ "$ENTERPRISES" -lt 1 ]; then
    echo "Недостаточно предприятий!"
    exit 1
fi

if [ "$VEHICLES" -lt 1 ]; then
    echo "Недостаточно автомобилей!"
    exit 1
fi

if [ "$DRIVERS" -lt 1 ]; then
    echo "Недостаточно водителей!"
    exit 1
fi

echo "Проверка данных пройдена успешно!"