# Docker - Инструкция по использованию

Подробное руководство по работе с Docker и Docker Compose для Learning Platform.

## 📦 Обзор сервисов

Проект состоит из следующих сервисов, которые можно запустить через Docker Compose:

- **postgres** - PostgreSQL 15 база данных
- **api** - ASP.NET Core Web API
- **client** - Blazor Server приложение
- **backgroundtasks** - Фоновые задачи (планируется)

## 🚀 Быстрый старт

### Запуск всех сервисов

```bash
# Запустить все сервисы в фоновом режиме
docker compose up -d

# Просмотр логов в реальном времени
docker compose logs -f

# Остановка всех сервисов
docker compose down
```

### Применение миграций

После первого запуска необходимо применить миграции базы данных:

```bash
# Убедитесь, что EF Core Tools установлены
dotnet tool install --global dotnet-ef

# Добавьте инструмент в PATH
export PATH="$PATH:$HOME/.dotnet/tools"

# Примените миграции (PostgreSQL должен быть доступен на localhost:5433)
dotnet ef database update --project LearningPlatform.Data --startup-project LearningPlatform.API
```

**Примечание:** При выполнении миграций вы можете увидеть `HostAbortedException` - это нормальное поведение, миграции применяются успешно.

## 🔌 Порты и доступ к сервисам

### Порты

- **PostgreSQL**: `5433` (внешний) → `5432` (внутри контейнера)
- **API**: `8080` (HTTP)
- **Client**: `5002` (HTTP), `5003` (HTTPS)
- **BackgroundTasks**: без внешних портов (фоновый сервис)

### Доступ к сервисам

- **API Swagger**: http://localhost:8080/swagger
- **Blazor Client**: http://localhost:5002

### Подключение к PostgreSQL

```bash
# Из хоста (локально)
psql -h localhost -p 5433 -U postgres -d learning_platform

# Из контейнера
docker compose exec postgres psql -U postgres -d learning_platform
```

Пароль по умолчанию: `postgres`

## 📋 Управление сервисами

### Запуск отдельных сервисов

```bash
# Запустить только PostgreSQL
docker compose up -d postgres

# Запустить PostgreSQL и API
docker compose up -d postgres api

# Запустить все сервисы
docker compose up -d
```

### Остановка сервисов

```bash
# Остановить все сервисы
docker compose down

# Остановить конкретный сервис
docker compose stop api

# Остановить и удалить контейнеры
docker compose down
```

### Перезапуск сервисов

```bash
# Перезапустить все сервисы
docker compose restart

# Перезапустить конкретный сервис
docker compose restart api
```

## 📊 Логи

### Просмотр логов через Docker Compose

```bash
# Все сервисы
docker compose logs -f

# Конкретный сервис
docker compose logs -f api
docker compose logs -f client
docker compose logs -f backgroundtasks
docker compose logs -f postgres

# Последние 50 строк логов
docker compose logs --tail=50 api

# Логи с временными метками
docker compose logs -f -t api
```

### Логи в файлах

Логи также сохраняются в локальные директории через volumes:
- `LearningPlatform.API/logs/` - логи API
- `LearningPlatform.Client/logs/` - логи Client
- `LearningPlatform.BackgroundTasks/logs/` - логи BackgroundTasks

Файлы логов имеют формат: `learningplatform-YYYYMMDD.txt`

## 🔨 Пересборка образов

### Пересборка всех образов

```bash
# Пересборка без кэша
docker compose build --no-cache

# Пересборка и запуск
docker compose up -d --build
```

### Пересборка конкретного сервиса

```bash
# Пересборка API
docker compose build api

# Пересборка Client
docker compose build client

# Пересборка и перезапуск
docker compose up -d --build api
```

## 🧹 Очистка

### Остановка и удаление контейнеров

```bash
# Остановка и удаление контейнеров
docker compose down

# Остановка, удаление контейнеров и volumes (удалит данные БД!)
docker compose down -v

# Удаление контейнеров, volumes и образов
docker compose down --rmi all -v
```

### Очистка Docker системы

```bash
# Удалить неиспользуемые образы
docker image prune -a

# Удалить неиспользуемые volumes
docker volume prune

# Полная очистка (осторожно!)
docker system prune -a --volumes
```

## 🐛 Отладка

### Вход в контейнер

```bash
# Войти в контейнер API
docker compose exec api bash

# Войти в контейнер Client
docker compose exec client bash

# Войти в контейнер BackgroundTasks
docker compose exec backgroundtasks bash

# Войти в контейнер PostgreSQL
docker compose exec postgres bash
```

### Проверка состояния

```bash
# Статус всех контейнеров
docker compose ps

# Детальная информация о контейнере
docker compose ps api

# Проверка логов конкретного сервиса
docker compose logs api --tail=50

# Проверка использования ресурсов
docker stats
```

### Выполнение команд в контейнере

```bash
# Выполнить команду в контейнере API
docker compose exec api dotnet --version

# Выполнить миграции внутри контейнера API
docker compose exec api bash -c "export PATH=\"\$PATH:\$HOME/.dotnet/tools\" && dotnet ef database update --project /src/LearningPlatform.Data --startup-project /src/LearningPlatform.API"
```

## ⚙️ Переменные окружения

Все переменные окружения настраиваются в `docker-compose.yml`. Основные:

### API (api)
- `ASPNETCORE_ENVIRONMENT` - окружение (Development/Production)
- `ASPNETCORE_URLS` - URL для приложения (по умолчанию: `http://+:8080`)
- `ConnectionStrings__DefaultConnection` - строка подключения к БД

### Client (client)
- `ASPNETCORE_ENVIRONMENT` - окружение (Development/Production)
- `ASPNETCORE_URLS` - URL для приложения (по умолчанию: `http://+:5000`)
- `ApiBaseUrl` - URL API для Client (по умолчанию: `http://api:8080`)

### BackgroundTasks (backgroundtasks)
- `ASPNETCORE_ENVIRONMENT` - окружение (Development/Production)
- `ConnectionStrings__DefaultConnection` - строка подключения к БД

### PostgreSQL (postgres)
- `POSTGRES_USER` - пользователь БД (по умолчанию: `postgres`)
- `POSTGRES_PASSWORD` - пароль БД (по умолчанию: `postgres`)
- `POSTGRES_DB` - имя БД (по умолчанию: `learning_platform`)

### Изменение переменных окружения

Вы можете переопределить переменные окружения через файл `.env` или напрямую в `docker-compose.yml`:

```yaml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Port=5432;Database=learning_platform;Username=postgres;Password=mysecurepassword
```

## 🌐 Сеть

Все сервисы находятся в одной Docker сети `learningplatform-network` и могут обращаться друг к другу по именам сервисов:

- `postgres` - PostgreSQL сервер
- `api` - API сервис
- `client` - Blazor Client
- `backgroundtasks` - Background Tasks

### Примеры подключения

```bash
# Из контейнера API к PostgreSQL
Host=postgres;Port=5432;Database=learning_platform;Username=postgres;Password=postgres

# Из контейнера Client к API
http://api:8080
```

## 💾 Volumes

### Постоянное хранение данных

- `pgdata` - данные PostgreSQL (хранятся в Docker volume)
- `./LearningPlatform.API/logs` - логи API (монтируются из хоста)
- `./LearningPlatform.Client/logs` - логи Client (монтируются из хоста)
- `./LearningPlatform.BackgroundTasks/logs` - логи BackgroundTasks (монтируются из хоста)

### Резервное копирование БД

```bash
# Создать резервную копию
docker compose exec postgres pg_dump -U postgres learning_platform > backup.sql

# Восстановить из резервной копии
docker compose exec -T postgres psql -U postgres learning_platform < backup.sql
```

## 🔍 Мониторинг

### Проверка здоровья сервисов

```bash
# Проверить статус всех контейнеров
docker compose ps

# Проверить логи на наличие ошибок
docker compose logs --tail=100 | grep -i error

# Проверить использование ресурсов
docker stats --no-stream
```

### Health checks

PostgreSQL имеет встроенный health check, который проверяет готовность БД каждые 10 секунд.

## 🚨 Решение проблем

### Проблема: Контейнер не запускается

```bash
# Проверить логи
docker compose logs api

# Проверить статус
docker compose ps

# Пересобрать образ
docker compose build --no-cache api
docker compose up -d api
```

### Проблема: Не могу подключиться к БД

```bash
# Проверить, что PostgreSQL запущен
docker compose ps postgres

# Проверить логи PostgreSQL
docker compose logs postgres

# Проверить подключение
docker compose exec postgres psql -U postgres -d learning_platform -c "SELECT 1;"
```

### Проблема: Порты заняты

Если порты уже заняты, измените их в `docker-compose.yml`:

```yaml
services:
  api:
    ports:
      - "8081:8080"  # Измените 8080 на 8081
```

### Проблема: Логи не сохраняются

Убедитесь, что директории логов существуют и имеют правильные права доступа:

```bash
mkdir -p LearningPlatform.API/logs
mkdir -p LearningPlatform.Client/logs
mkdir -p LearningPlatform.BackgroundTasks/logs
```

## 📝 Полезные команды

```bash
# Просмотр всех образов
docker images | grep learningplatform

# Просмотр всех контейнеров (включая остановленные)
docker ps -a | grep learningplatform

# Просмотр volumes
docker volume ls | grep learningplatform

# Просмотр сетей
docker network ls | grep learningplatform

# Очистка всего (осторожно!)
docker compose down -v --rmi all
docker system prune -a --volumes
```
