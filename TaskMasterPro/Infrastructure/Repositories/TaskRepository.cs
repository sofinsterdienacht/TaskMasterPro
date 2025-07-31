using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Infrastructure.Data;

namespace TaskMasterPro.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _context;

    public TaskRepository(TaskDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await _context.Tasks.ToListAsync();
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByPriorityAsync(TaskPriority priority)
    {
        return await _context.Tasks.Where(t => t.Priority == priority).ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status)
    {
        return await _context.Tasks.Where(t => t.Status == status).ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByCategoryAsync(TaskCategory category)
    {
        return await _context.Tasks.Where(t => t.Category == category).ToListAsync();
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem task)
    {
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetCompletedTasksCountAsync()
    {
        return await _context.Tasks.CountAsync(t => t.Status == TaskItemStatus.Completed);
    }

    public async Task<int> GetPendingTasksCountAsync()
    {
        return await _context.Tasks.CountAsync(t => t.Status == TaskItemStatus.Pending);
    }
    
    public async Task<int> GetInProgressTasksCountAsync()
    {
        return await _context.Tasks.CountAsync(t => t.Status == TaskItemStatus.InProgress);
    }
}