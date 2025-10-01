using TaskMasterPro.Core.Entities;

namespace TaskMasterPro.Core.Interfaces;

public interface ICategoryService
{








    Dictionary<TaskCategory, string[]> GetAllCategories();








    Dictionary<TaskPriority, string[]> GetAllPriorities();










    TaskCategory DetermineCategory(string title);








    /// <returns>Приоритет задачи</returns>
    TaskPriority DeterminePriority(string title);
}