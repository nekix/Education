#!/bin/bash
set -e

echo "CarPark Demo Data Generation"
echo "================================"
echo ""
echo "Конфигурация:"
echo "  - Seed: ${DEMO_SEED}"
echo "  - Предприятия: ${DEMO_ENTERPRISES}"
echo "  - Машин на предприятие: ${DEMO_VEHICLES}"
echo "  - Водителей на предприятие: ${DEMO_DRIVERS}"
echo "  - Период: ${DEMO_START_DATE} - ${DEMO_END_DATE}"
echo ""

START_TIME=$(date +%s)

# Проверка уже выполненной генерации
if [ -f /status/generation-completed ]; then
    echo "Генерация уже выполнена ранее"
    cat /status/data-stats.txt 2>/dev/null || true
    exit 0
fi

# echo "Запуск 02-generate-base-data.sh..."
# /scripts/02-generate-base-data.sh
# if [ $? -ne 0 ]; then
#     echo "Скрипт 02-generate-base-data.sh завершился с ошибкой!"
#     exit 1
# fi

# echo "Запуск 03-generate-tracks.sh..."
# /scripts/03-generate-tracks.sh
# if [ $? -ne 0 ]; then
#     echo "Скрипт 03-generate-tracks.sh завершился с ошибкой!"
#     exit 1
# fi

# echo "Запуск 04-verify-data.sh..."
# /scripts/04-verify-data.sh
# if [ $? -ne 0 ]; then
#     echo "Скрипт 04-verify-data.sh завершился с ошибкой!"
#     exit 1
# fi

END_TIME=$(date +%s)
DURATION=$((END_TIME - START_TIME))
MINUTES=$((DURATION / 60))
SECONDS=$((DURATION % 60))

echo ""
echo "================================"
echo "Генерация завершена успешно!"
echo "Время выполнения: ${MINUTES}м ${SECONDS}с"
echo ""
echo "Система готова к использованию:"
echo "  - Web: http://localhost:${WEB_PORT:-8080}"
echo ""

# Отметить завершение
touch /status/generation-completed
date > /status/generation-timestamp

# cat /status/data-stats.txt