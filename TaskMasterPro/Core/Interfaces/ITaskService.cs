

using TaskMasterPro.Core.Entities;

namespace TaskMasterPro.Core.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetAllTasksAsync();
    Task<TaskItem?> GetTaskByIdAsync(int id);
    Task<TaskItem> CreateTaskAsync(TaskItem task);
    Task<TaskItem> UpdateTaskAsync(TaskItem task);
    Task DeleteTaskAsync(int id);
    Task<TaskItem> CompleteTaskAsync(int id);
    Task<TaskItem> StartTaskAsync(int id);
    Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority);
    Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status);
    Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category);
    Task<object> GetStatisticsAsync();
}