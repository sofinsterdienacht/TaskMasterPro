using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;
using System.Linq;

namespace TaskMasterPro.Core.Services;

public class CategoryService : ICategoryService
{
    // Словарь с ключевыми словами для каждой категории
    private readonly Dictionary<TaskCategory, string[]> _categoryKeywords = new()
    {
        { TaskCategory.Work, new[] { "работ", "work", "проект", "клиент", "презентац", "presentation", 
                                    "отчет", "report", "совещан", "meeting", "конференц", "conference" } },
        { TaskCategory.Study, new[] { "учеб", "study", "экзамен", "курс" } },
        { TaskCategory.Health, new[] { "здоров", "health", "врач", "спорт" } },
        { TaskCategory.Finance, new[] { "денег", "finance", "бюджет", "счет" } },
        { TaskCategory.Shopping, new[] { "покуп", "shopping", "купи", "магазин" } },
        { TaskCategory.Personal, new[] { "личн", "personal", "дом", "семья" } }
    };
    
    // Словарь с ключевыми словами для каждого приоритета
    private readonly Dictionary<TaskPriority, string[]> _priorityKeywords = new()
    {
        { TaskPriority.High, new[] { "срочно", "urgent", "критично", "важно" } },
        { TaskPriority.Medium, new[] { "средний", "medium", "нормально" } },
        { TaskPriority.Low, new[] { "не важно", "потом", "low" } },
        { TaskPriority.Urgent, new[] { "критично", "urgent", "немедленно" } }
    };

    // Плоские массивы всех ключевых слов (для быстрого поиска совпадений одним проходом)
    private readonly string[] _allCategoryKeywords;
    private readonly string[] _allPriorityKeywords;

    // Отображение ключевое слово -> категория/приоритет (для быстрого определения результата)
    private readonly Dictionary<string, TaskCategory> _keywordToCategory;
    private readonly Dictionary<string, TaskPriority> _keywordToPriority;

    public CategoryService()
    {
        // Построение плоских коллекций и отображений
        _keywordToCategory = new Dictionary<string, TaskCategory>();
        foreach (var pair in _categoryKeywords)
        {
            foreach (var keyword in pair.Value)
            {
                // Последнее вхождение переопределяет предыдущее, что упрощает поддержку
                _keywordToCategory[keyword] = pair.Key;
            }
        }
        _allCategoryKeywords = _keywordToCategory.Keys.Distinct().ToArray();

        _keywordToPriority = new Dictionary<string, TaskPriority>();
        foreach (var pair in _priorityKeywords)
        {
            foreach (var keyword in pair.Value)
            {
                // Не переопределяем ранее заданные ключевые слова,
                // чтобы приоритет оставался за первым определением
                if (!_keywordToPriority.ContainsKey(keyword))
                {
                    _keywordToPriority[keyword] = pair.Key;
                }
            }
        }
        _allPriorityKeywords = _keywordToPriority.Keys.Distinct().ToArray();
    }
    
    /// <summary>
    /// Получает все доступные категории задач
    /// </summary>
    /// <returns>Словарь категорий с ключевыми словами</returns>
    public Dictionary<TaskCategory, string[]> GetAllCategories() => _categoryKeywords;
    
    /// <summary>
    /// Получает все доступные приоритеты задач
    /// </summary>
    /// <returns>Словарь приоритетов с ключевыми словами</returns>
    public Dictionary<TaskPriority, string[]> GetAllPriorities() => _priorityKeywords;
    
    /// <summary>
    /// Определяет категорию задачи по её названию
    /// </summary>
    /// <param name="title">Название задачи</param>
    /// <returns>Категория задачи</returns>
    public TaskCategory DetermineCategory(string title)
    {
        var lowerTitle = title.ToLower();
        
        // Поиск совпадения в едином массиве ключевых слов
        foreach (var keyword in _allCategoryKeywords)
        {
            if (lowerTitle.Contains(keyword))
            {
                // Возвращаем категорию, соответствующую найденному ключевому слову
                return _keywordToCategory[keyword];
            }
        }

        // Если ни одна категория не подошла, возвращаем Personal
        return TaskCategory.Personal;
    }
    
    /// <summary>
    /// Определяет приоритет задачи по её названию
    /// </summary>
    /// <param name="title">Название задачи</param>
    /// <returns>Приоритет задачи</returns>
    public TaskPriority DeterminePriority(string title)
    {
        var lowerTitle = title.ToLower();
        
        // Поиск совпадения в едином массиве ключевых слов
        foreach (var keyword in _allPriorityKeywords)
        {
            if (lowerTitle.Contains(keyword))
            {
                return _keywordToPriority[keyword];
            }
        }

        // Если ни один приоритет не подошел, возвращаем Medium
        return TaskPriority.Medium;
    }
}