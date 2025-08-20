# TaskMasterPro

## Конфигурация

Проект теперь использует централизованную конфигурацию через `appsettings.json` файлы.

### Структура конфигурации

#### API Settings
- `BaseUrl` - базовый URL для API (по умолчанию: `http://localhost:5194`)
- `TasksEndpoint` - эндпоинт для задач (по умолчанию: `/api/tasks`)
- `CategoriesEndpoint` - эндпоинт для категорий (по умолчанию: `/api/categories`)

#### Logging Settings
- `TaskServiceLogFile` - файл логов для TaskService (по умолчанию: `task_service.log`)
- `CategoryServiceLogFile` - файл логов для CategoryService (по умолчанию: `category_service.log`)
- `MainViewModelLogFile` - файл логов для MainViewModel (по умолчанию: `main_viewmodel.log`)
- `DebugLogFile` - файл отладочных логов (по умолчанию: `logs/wpf_debug.log`)

### Файлы конфигурации

#### API проект (`TaskMasterPro/`)
- `appsettings.json` - основная конфигурация
- `appsettings.Development.json` - конфигурация для разработки

#### WPF проект (`TaskMasterPro.WPF/`)
- `appsettings.json` - основная конфигурация
- `appsettings.Development.json` - конфигурация для разработки

### Изменения в коде

1. **TaskService** - теперь принимает `ConfigurationService` в конструкторе
2. **CategoryService** - теперь принимает `ConfigurationService` в конструкторе
3. **MainViewModel** - теперь использует `ConfigurationService` для получения настроек
4. **ConfigurationService** - новый сервис для работы с конфигурацией

### Преимущества новой конфигурации

1. **Централизация** - все настройки в одном месте
2. **Гибкость** - легко изменять настройки без перекомпиляции
3. **Окружения** - разные настройки для разных окружений (Development, Production)
4. **Типизация** - строго типизированный доступ к настройкам
5. **Перезагрузка** - автоматическая перезагрузка конфигурации при изменении файлов

### Пример использования

```csharp
// В сервисах
public class TaskService
{
    public TaskService(ConfigurationService configurationService)
    {
        var apiSettings = configurationService.GetApiSettings();
        var loggingSettings = configurationService.GetLoggingSettings();
        
        _baseUrl = $"{apiSettings.BaseUrl}{apiSettings.TasksEndpoint}";
        _logFile = loggingSettings.TaskServiceLogFile;
    }
}

// В ViewModel
public class MainViewModel
{
    public MainViewModel()
    {
        _configurationService = new ConfigurationService();
        var loggingSettings = _configurationService.GetLoggingSettings();
        
        _logFile = loggingSettings.MainViewModelLogFile;
        _debugLogFile = loggingSettings.DebugLogFile;
    }
}
```

### Запуск

1. Убедитесь, что API проект запущен на порту 5194
2. Запустите WPF приложение
3. Приложение автоматически загрузит конфигурацию из `appsettings.json`

### Изменение настроек

Для изменения настроек отредактируйте соответствующий `appsettings.json` файл. Изменения вступят в силу при следующем запуске приложения.

