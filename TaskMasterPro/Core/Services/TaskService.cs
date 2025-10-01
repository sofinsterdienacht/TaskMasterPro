


using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;

namespace TaskMasterPro.Core.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly ICategoryService _categoryService;

    public TaskService(ITaskRepository taskRepository, ICategoryService categoryService)
    {
        _taskRepository = taskRepository;
        _categoryService = categoryService;
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


        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Название задачи не может быть пустым");


        task.Priority = DeterminePriority(task.Title);


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
        return _categoryService.DeterminePriority(title);
    }

    private TaskCategory DetermineCategory(string title)
    {
        return _categoryService.DetermineCategory(title);
    }
}