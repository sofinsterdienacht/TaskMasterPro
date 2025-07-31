


using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;

namespace TaskMasterPro.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async Task<TaskItem?> GetTaskByIdAsync(int id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async Task<TaskItem> CreateTaskAsync(TaskItem task)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Название задачи не может быть пустым");

        // Автоматическое определение приоритета
        task.Priority = DeterminePriority(task.Title);
        
        // Автоматическое определение категории
        task.Category = DetermineCategory(task.Title);

        return await _taskRepository.AddAsync(task);
    }

    public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Название задачи не может быть пустым");

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task DeleteTaskAsync(int id)
    {
        await _taskRepository.DeleteAsync(id);
    }

    public async Task<TaskItem> CompleteTaskAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new ArgumentException("Задача не найдена");

        task.Status = TaskItemStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<TaskItem> StartTaskAsync(int id)
    {
        var task = await _taskRepository.GetByIdAsync(id);
        if (task == null)
            throw new ArgumentException("Задача не найдена");

        task.Status = TaskItemStatus.InProgress;

        return await _taskRepository.UpdateAsync(task);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByPriorityAsync(TaskPriority priority)
    {
        return await _taskRepository.GetByPriorityAsync(priority);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByStatusAsync(TaskItemStatus status)
    {
        return await _taskRepository.GetByStatusAsync(status);
    }

    public async Task<IEnumerable<TaskItem>> GetTasksByCategoryAsync(TaskCategory category)
    {
        return await _taskRepository.GetByCategoryAsync(category);
    }

    public async Task<object> GetStatisticsAsync()
    {
        var completedCount = await _taskRepository.GetCompletedTasksCountAsync();
        var pendingCount = await _taskRepository.GetPendingTasksCountAsync();
        var inProgressCount = await _taskRepository.GetInProgressTasksCountAsync();
        var totalCount = completedCount + pendingCount + inProgressCount;

        return new
        {
            TotalTasks = totalCount,
            CompletedTasks = completedCount,
            PendingTasks = pendingCount,
            InProgressTasks = inProgressCount,
            CompletionRate = totalCount > 0 ? (double)completedCount / totalCount * 100 : 0
        };
    }

    private TaskPriority DeterminePriority(string title)
    {
        var lowerTitle = title.ToLower();
        
        if (lowerTitle.Contains("срочно") || lowerTitle.Contains("urgent") || 
            lowerTitle.Contains("критично") || lowerTitle.Contains("важно"))
            return TaskPriority.High;
        
        if (lowerTitle.Contains("не важно") || lowerTitle.Contains("потом"))
            return TaskPriority.Low;
        
        return TaskPriority.Medium;
    }

    private TaskCategory DetermineCategory(string title)
    {
        var lowerTitle = title.ToLower();
        
        if (lowerTitle.Contains("работ") || lowerTitle.Contains("work") || 
            lowerTitle.Contains("проект") || lowerTitle.Contains("клиент") ||
            lowerTitle.Contains("презентац") || lowerTitle.Contains("presentation") ||
            lowerTitle.Contains("отчет") || lowerTitle.Contains("report") ||
            lowerTitle.Contains("совещан") || lowerTitle.Contains("meeting") ||
            lowerTitle.Contains("конференц") || lowerTitle.Contains("conference"))
            return TaskCategory.Work;
        
        if (lowerTitle.Contains("учеб") || lowerTitle.Contains("study") || 
            lowerTitle.Contains("экзамен") || lowerTitle.Contains("курс"))
            return TaskCategory.Study;
        
        if (lowerTitle.Contains("здоров") || lowerTitle.Contains("health") || 
            lowerTitle.Contains("врач") || lowerTitle.Contains("спорт"))
            return TaskCategory.Health;
        
        if (lowerTitle.Contains("денег") || lowerTitle.Contains("finance") || 
            lowerTitle.Contains("бюджет") || lowerTitle.Contains("счет"))
            return TaskCategory.Finance;
        
        if (lowerTitle.Contains("покуп") || lowerTitle.Contains("shopping") || 
            lowerTitle.Contains("купи") || lowerTitle.Contains("магазин"))
            return TaskCategory.Shopping;
        
        return TaskCategory.Personal;
    }
}