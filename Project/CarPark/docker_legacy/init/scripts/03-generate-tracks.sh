#!/bin/bash
set -e

echo "🚗 Генерация GPS треков и поездок..."

# Проверка, что треки еще не сгенерированы
if [ -f /status/tracks-completed ]; then
    echo "Треки уже сгенерированы ранее"
    exit 0
fi

# Проверка API ключа
if [ -z "$GRAPHHOPPER_API_KEY" ]; then
    echo "GRAPHHOPPER_API_KEY не установлен!"
    echo "Пропускаем генерацию треков..."
    echo "Для генерации треков получите ключ на https://www.graphhopper.com/"
    touch /status/tracks-skipped
    exit 0
fi

# Проверка файла с vehicle IDs
if [ ! -f /tmp/vehicles.txt ]; then
    echo "Файл /tmp/vehicles.txt не найден!"
    exit 1
fi

# Проверка, что уже есть GPS точки (треки уже генерировались)
export PGPASSWORD=$POSTGRES_PASSWORD
EXISTING_GEO_POINTS=$(psql -h "postgres" -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -t -c "SELECT COUNT(*) FROM vehicle_geo_time_points;" 2>/dev/null || echo "0")
if [ "$EXISTING_GEO_POINTS" -gt 0 ]; then
    echo "GPS треки уже существуют: $EXISTING_GEO_POINTS точек"
    echo "Пропускаем генерацию треков..."
    touch /status/tracks-completed
    exit 0
fi

VEHICLE_COUNT=$(wc -l < /tmp/vehicles.txt)
echo "Будет сгенерировано треков для ${VEHICLE_COUNT} автомобилей"
echo "Это может занять 30-60 минут..."
echo ""

cd /app

# Генерация треков
dotnet /app/CarPark.TrackGenerator/CarPark.TrackGenerator.dll \
    generate-bulk \
    --vehicle-ids-file /tmp/vehicles.txt \
    --start-date ${DEMO_START_DATE:-2025-10-01} \
    --end-date ${DEMO_END_DATE:-2025-11-05} \
    --active-days-ratio 0.7 \
    --min-avg-daily-distance 50 \
    --max-avg-daily-distance 200 \
    --batch-size 1000 \
    --center-lat 55.7558 \
    --center-lon 37.6176 \
    --radius-km 50 \
    --max-speed 120 \
    --min-speed 10 \
    --max-acceleration 12000 \
    --point-interval 30 \
    --interval-variation 10 \
    --connection-string "Host=postgres;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};Timezone=UTC" \
    --graphhopper-key "${GRAPHHOPPER_API_KEY}"

if [ $? -ne 0 ]; then
    echo "Ошибка при генерации треков!"
    exit 1
fi

echo ""
echo "Треки и поездки сгенерированы!"
touch /status/tracks-completed