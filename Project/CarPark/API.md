# API Документация

## Доступ к документации

### Swagger UI
В режиме разработки доступен Swagger UI:
- **URL**: `https://localhost:{port}/swagger`
- **OpenAPI спецификация**: `https://localhost:{port}/openapi/v1.json`

### Аутентификация в API
Все API эндпоинты (кроме `/api/auth/login`) требуют аутентификации через cookie.

## Эндпоинты авторизации

### POST /api/auth/login
Авторизация пользователя в API.

**Запрос:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Ответы:**
- `201 Created` - успешная авторизация
- `401 Unauthorized` - неверные учетные данные

### POST /api/auth/logout
Выход из системы.

**Требует:** CSRF токен
**Ответы:**
- `204 No Content` - успешный выход

## CSRF защита

### GET /api/csrf
Получение CSRF токена для защищенных операций.

**Требует:** Авторизация менеджера
**Ответ:**
```json
{
  "token": "string"
}
```

## API Автомобилей

**Базовый путь:** `/api/vehicles`
**Требует:** Авторизация менеджера

### GET /api/vehicles
Получить список автомобилей, доступных текущему менеджеру.

**Ответ:** `200 OK`
```json
[
  {
    "id": 1,
    "modelId": 1,
    "enterpriseId": 1,
    "vinNumber": "string",
    "price": 0,
    "manufactureYear": 2024,
    "mileage": 0,
    "color": "string",
    "driversAssignments": {
      "driversIds": [1, 2],
      "activeDriverId": 1
    }
  }
]
```

### GET /api/vehicles/{id}
Получить конкретный автомобиль.

**Ответы:**
- `200 OK` - автомобиль найден
- `404 Not Found` - автомобиль не найден или недоступен менеджеру

### POST /api/vehicles
Создать новый автомобиль.

**Требует:** CSRF токен
**Запрос:**
```json
{
  "modelId": 1,
  "enterpriseId": 1,
  "vinNumber": "string",
  "price": 0,
  "manufactureYear": 2024,
  "mileage": 0,
  "color": "string",
  "driversAssignments": {
    "driversIds": [1, 2],
    "activeDriverId": 1
  }
}
```

**Ответы:**
- `201 Created` - автомобиль создан
- `400 Bad Request` - некорректные данные

### PUT /api/vehicles/{id}
Обновить автомобиль.

**Требует:** CSRF токен
**Запрос:** Аналогичен POST запросу
**Ответы:**
- `204 No Content` - успешно обновлен
- `400 Bad Request` - некорректные данные
- `404 Not Found` - автомобиль не найден

### DELETE /api/vehicles/{id}
Удалить автомобиль.

**Требует:** CSRF токен
**Ответы:**
- `204 No Content` - успешно удален
- `400 Bad Request` - некорректный запрос
- `404 Not Found` - автомобиль не найден
- `409 Conflict` - нарушение логической консистентности

## API Моделей автомобилей

**Базовый путь:** `/api/models`

### GET /api/models
Получить список всех моделей автомобилей.

**Ответ:** `200 OK`
```json
[
  {
    "id": 1,
    "modelName": "string",
    "vehicleType": "string",
    "seatsCount": 5,
    "maxLoadingWeightKg": 1000.0,
    "enginePowerKW": 100.0,
    "transmissionType": "string",
    "fuelSystemType": "string",
    "fuelTankVolumeLiters": "50"
  }
]
```

### GET /api/models/{id}
Получить конкретную модель.

**Ответы:**
- `200 OK` - модель найдена
- `404 Not Found` - модель не найдена

### POST /api/models
Создать новую модель.

**Требует:** CSRF токен
**Запрос:**
```json
{
  "modelName": "string",
  "vehicleType": "string",
  "seatsCount": 5,
  "maxLoadingWeightKg": 1000.0,
  "enginePowerKW": 100.0,
  "transmissionType": "string",
  "fuelSystemType": "string",
  "fuelTankVolumeLiters": "50"
}
```

**Ответы:**
- `201 Created` - модель создана
- `400 Bad Request` - некорректные данные

### PUT /api/models/{id}
Обновить модель.

**Требует:** CSRF токен
**Ответы:**
- `204 No Content` - успешно обновлена
- `400 Bad Request` - некорректные данные
- `404 Not Found` - модель не найдена

### DELETE /api/models/{id}
Удалить модель.

**Требует:** CSRF токен
**Ответы:**
- `204 No Content` - успешно удалена
- `400 Bad Request` - некорректный запрос
- `404 Not Found` - модель не найдена
- `409 Conflict` - модель используется в автомобилях

## API Водителей

**Базовый путь:** `/api/drivers`
**Требует:** Авторизация менеджера
**Доступ:** Только чтение

### GET /api/drivers
Получить список водителей, доступных текущему менеджеру.

**Ответ:** `200 OK`
```json
[
  {
    "id": 1,
    "enterpriseId": 1,
    "fullName": "string",
    "driverLicenseNumber": "string",
    "vehiclesAssignments": {
      "vehiclesIds": [1, 2],
      "activeVehicleId": 1
    }
  }
]
```

### GET /api/drivers/{id}
Получить конкретного водителя.

**Ответы:**
- `200 OK` - водитель найден
- `404 Not Found` - водитель не найден или недоступен менеджеру

## API Предприятий

**Базовый путь:** `/api/enterprises`
**Требует:** Авторизация менеджера
**Доступ:** Только чтение

### GET /api/enterprises
Получить список предприятий, управляемых текущим менеджером.

**Ответ:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "string",
    "legalAddress": "string",
    "relatedEntities": {
      "driversIds": [1, 2, 3],
      "vehiclesIds": [1, 2, 3]
    }
  }
]
```

### GET /api/enterprises/{id}
Получить конкретное предприятие.

**Ответы:**
- `200 OK` - предприятие найдено
- `404 Not Found` - предприятие не найдено или недоступно менеджеру

### DELETE /api/enterprises/{id}
Удалить предприятие.

**Требует:** CSRF токен
**Ответы:**
- `204 No Content` - предприятие успешно удалено
- `400 Bad Request` - некорректный запрос
- `403 Forbidden` - нет доступа к предприятию
- `404 Not Found` - предприятие не найдено
- `409 Conflict` - нарушение бизнес-правил

**Бизнес-правила для удаления:**
- Предприятие должно быть доступно запрашивающему менеджеру
- Предприятие не должно быть видимо другим менеджерам
- В предприятии не должно быть автомобилей
- В предприятии не должно быть водителей

## Коды ответов

### Стандартные коды по HTTP методам
- **GET**: 200, 404, 401, 403
- **POST**: 201, 400, 401, 403, 409
- **PUT**: 200, 204, 400, 401, 403, 404, 409
- **DELETE**: 204, 400, 401, 403, 404, 409

### Описание кодов
- **200 OK** - успешный запрос с данными
- **201 Created** - ресурс успешно создан
- **204 No Content** - успешная операция без возврата данных
- **400 Bad Request** - некорректный запрос
- **401 Unauthorized** - требуется аутентификация
- **403 Forbidden** - доступ запрещен
- **404 Not Found** - ресурс не найден
- **409 Conflict** - конфликт данных/нарушение консистентности

## Особенности API

### Фильтрация по менеджеру
Менеджеры видят только:
- Автомобили из управляемых предприятий
- Водителей из управляемых предприятий  
- Предприятия, которыми они управляют

### Связанные сущности
Связанные сущности возвращаются в виде массивов ID:
- `driversIds` - массив ID назначенных водителей
- `vehiclesIds` - массив ID назначенных автомобилей
- `activeDriverId` / `activeVehicleId` - ID активного назначения

### CSRF защита
Все изменяющие операции (POST, PUT, DELETE) требуют CSRF токен в заголовке `RequestVerificationToken`.
