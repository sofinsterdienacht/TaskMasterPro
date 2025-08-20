using TaskMasterPro.Core.Entities;

namespace TaskMasterPro.Core.Interfaces;

public interface ICategoryService
{
    /// <summary>
    /// Получает все доступные категории задач
    /// </summary>
    /// <returns>Словарь категорий с ключевыми словами</returns>
    Dictionary<TaskCategory, string[]> GetAllCategories();
    
    /// <summary>
    /// Получает все доступные приоритеты задач
    /// </summary>
    /// <returns>Словарь приоритетов с ключевыми словами</returns>
    Dictionary<TaskPriority, string[]> GetAllPriorities();
    
    /// <summary>
    /// Определяет категорию задачи по её названию
    /// </summary>
    /// <param name="title">Название задачи</param>
    /// <returns>Категория задачи</returns>
    TaskCategory DetermineCategory(string title);
    
    /// <summary>
    /// Определяет приоритет задачи по её названию
    /// </summary>
    /// <param name="title">Название задачи</param>
    /// <returns>Приоритет задачи</returns>
    TaskPriority DeterminePriority(string title);
}