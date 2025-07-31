


using TaskMasterPro.Core.Entities;

namespace TaskMasterPro.Core.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<IEnumerable<TaskItem>> GetByPriorityAsync(TaskPriority priority);
    Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status);
    Task<IEnumerable<TaskItem>> GetByCategoryAsync(TaskCategory category);
    Task<TaskItem> AddAsync(TaskItem task);
    Task<TaskItem> UpdateAsync(TaskItem task);
    Task DeleteAsync(int id);
    Task<int> GetCompletedTasksCountAsync();
    Task<int> GetPendingTasksCountAsync();
    Task<int> GetInProgressTasksCountAsync();
}