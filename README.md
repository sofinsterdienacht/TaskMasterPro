 
# TaskMaster Pro 🎯

Умное приложение для управления задачами с автоматической классификацией и AI-помощником.

![img.png](img.png)


## ✨ Особенности

- 🤖 **Автоматическая классификация** - AI определяет категорию и приоритет по названию задачи
- 🎨 **Двойной интерфейс** - Web API + WPF desktop приложение
- 📊 **Статистика и аналитика** - отслеживание прогресса по категориям и приоритетам
- 🐳 **Docker поддержка** - легкий запуск всей инфраструктуры
- 🗄️ **Multi-database** - поддержка PostgreSQL и SQL Server
- 🔍 **Умный поиск** - фильтрация по категориям, статусам, приоритетам

## 🏗️ Архитектура

```
TaskMasterPro/
├── 📱 TaskMasterPro.WPF/          # WPF клиент (MVVM)
├── 🌐 TaskMasterPro/              # Web API (ASP.NET Core)
│   ├── Core/                      # Бизнес-логика
│   │   ├── Entities/              # Domain models
│   │   ├── Interfaces/            # Contracts
│   │   └── Services/              # Business logic
│   ├── Infrastructure/            # Data Access
│   │   ├── Data/                  # DbContext
│   │   └── Repositories/          # Repository pattern
│   └── API/                       # Presentation layer
│       ├── Controllers/           # REST API
│       └── DTOs/                  # Data Transfer Objects
├── 🧪 Tests/                      # Unit & Integration tests
└── 🐳 docker-compose.yml         # Docker конфигурация
```

## 🚀 Быстрый старт

### Вариант 1: Docker (Рекомендуется)
```bash
# Клонируй репозиторий
git clone <repository-url>
cd TaskMasterPro

# Запусти всю инфраструктуру
docker-compose up -d

# Проверь статус
docker-compose ps
```

После запуска API будет доступно на: http://localhost:5000

### Вариант 2: Локальная разработка
```bash
# Backend API
cd TaskMasterPro
dotnet run

# Frontend WPF (в отдельном терминале)
cd TaskMasterPro.WPF  
dotnet run
```

## 📡 API Endpoints

| Метод | Endpoint | Описание |
|-------|----------|----------|
| `GET` | `/api/tasks` | Получить все задачи |
| `POST` | `/api/tasks` | Создать новую задачу |
| `GET` | `/api/tasks/{id}` | Получить задачу по ID |
| `PUT` | `/api/tasks/{id}` | Обновить задачу |
| `DELETE` | `/api/tasks/{id}` | Удалить задачу |
| `POST` | `/api/tasks/{id}/complete` | Завершить задачу |
| `POST` | `/api/tasks/{id}/start` | Начать выполнение задачи |
| `GET` | `/api/tasks/priority/{priority}` | Задачи по приоритету |
| `GET` | `/api/tasks/status/{status}` | Задачи по статусу |
| `GET` | `/api/tasks/category/{category}` | Задачи по категории |
| `GET` | `/api/tasks/statistics` | Статистика задач |
| `GET` | `/api/categories` | Все категории |
| `GET` | `/api/categories/priorities` | Все приоритеты |
| `GET` | `/api/categories/all-enum-values` | Все значения перечислений |

## 🤖 AI Классификация задач

Приложение автоматически определяет категорию и приоритет по ключевым словам в названии задачи:

### 🗂️ Категории
| Категория | Ключевые слова |
|-----------|----------------|
| **Work** | "работа", "проект", "клиент", "презентация", "отчет", "совещание" |
| **Study** | "учеба", "экзамен", "курс", "study", "learning" |
| **Health** | "здоровье", "врач", "спорт", "health", "doctor" |
| **Shopping** | "покупка", "магазин", "купить", "shopping", "buy" |
| **Finance** | "деньги", "бюджет", "счет", "finance", "budget" |
| **Personal** | "личный", "дом", "семья", "personal", "home" |

### ⚡ Приоритеты
| Приоритет | Ключевые слова |
|-----------|----------------|
| **Urgent** | "критично", "urgent", "немедленно", "срочно" |
| **High** | "важно", "high", "приоритетно", "critical" |
| **Medium** | "средний", "medium", "нормально" |
| **Low** | "не важно", "low", "потом", "later" |

## ⚙️ Конфигурация

### Базы данных
Приложение поддерживает несколько провайдеров баз данных. Настройка в `appsettings.json`:

```json
{
  "DatabaseProvider": "Postgres", // или "SqlServer"
  "ConnectionStrings": {
    "Postgres": "Host=localhost;Port=5432;Database=taskmasterdb;Username=taskuser;Password=taskpass",
    "SqlServer": "Server=(localdb)\\mssqllocaldb;Database=TaskMasterDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

### Docker окружение
```yaml
environment:
  - DatabaseProvider=Postgres
  - ConnectionStrings__Postgres=Host=postgres;Database=taskmasterdb;Username=taskuser;Password=taskpass
```

## 🧪 Тестирование

```bash
# Unit tests
dotnet test Tests/TaskMasterPro.UnitTests/

# Integration tests  
dotnet test Tests/TaskMasterPro.IntegrationTests/

# Все тесты
dotnet test
```

### Примеры тестов:
- **CategoryServiceTests** - тестирование AI классификации
- **TasksApiTests** - интеграционные тесты API

## 🐛 Troubleshooting

### Проблемы с базой данных
```bash
# Принудительный перезапуск
docker-compose down
docker-compose up -d

# Ручное выполнение миграций
docker-compose exec api dotnet ef database update

# Просмотр логов базы данных
docker-compose logs postgres
```

### Проблемы с подключением WPF
Убедись что URL API указан правильно в `TaskMasterPro.WPF/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000",
    "TasksEndpoint": "/api/tasks",
    "CategoriesEndpoint": "/api/categories"
  }
}
```

### Проблемы с портами
Если порты заняты, измени в `docker-compose.yml`:
```yaml
ports:
  - "8080:8080"  # вместо 5000:8080
  - "8081:8081"  # вместо 5001:8081
```

## 🔧 Технические детали

### Технологии
- **Backend**: ASP.NET Core 8.0, Entity Framework Core
- **Frontend**: WPF, MVVM Pattern
- **Database**: PostgreSQL, SQL Server
- **Testing**: xUnit, WebApplicationFactory
- **Containerization**: Docker, Docker Compose

### Миграции базы данных
```bash
# Создание миграции
dotnet ef migrations add InitialCreate --project TaskMasterPro

# Применение миграций
dotnet ef database update --project TaskMasterPro
```

### Структура задачи (TaskItem)
```csharp
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskCategory Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
}
```

## 🤝 Разработка

### Требования
- .NET 8.0 SDK
- Docker & Docker Compose
- PostgreSQL 15+ или SQL Server
- IDE: Visual Studio 2022, Rider, или VS Code

### Начало разработки
1. Клонируй репозиторий
2. Запусти базу данных: `docker-compose up postgres -d`
3. Запусти API: `cd TaskMasterPro && dotnet run`
4. Запусти WPF: `cd TaskMasterPro.WPF && dotnet run`

### Структура репозитория
```
├── .github/workflows/    # CI/CD pipelines
├── TaskMasterPro/        # Web API (ASP.NET Core)
├── TaskMasterPro.WPF/    # WPF Desktop Client
├── Tests/               # Unit & Integration tests
│   ├── TaskMasterPro.UnitTests/
│   └── TaskMasterPro.IntegrationTests/
├── docker-compose.yml   # Docker configuration
└── README.md           # This file
```
 