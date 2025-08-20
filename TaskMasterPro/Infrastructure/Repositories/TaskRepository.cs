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
        var priorityId = (int)priority;
        return await _context.Tasks.Where(t => t.PriorityId == priorityId).ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAsync(TaskItemStatus status)
    {
        var statusId = (int)status;
        return await _context.Tasks.Where(t => t.StatusId == statusId).ToListAsync();
    }

    public async Task<IEnumerable<TaskItem>> GetByCategoryAsync(TaskCategory category)
    {
        var categoryId = (int)category;
        return await _context.Tasks.Where(t => t.CategoryId == categoryId).ToListAsync();
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
        var completedId = (int)TaskItemStatus.Completed;
        return await _context.Tasks.CountAsync(t => t.StatusId == completedId);
    }

    public async Task<int> GetPendingTasksCountAsync()
    {
        var pendingId = (int)TaskItemStatus.Pending;
        return await _context.Tasks.CountAsync(t => t.StatusId == pendingId);
    }
    
    public async Task<int> GetInProgressTasksCountAsync()
    {
        var inProgressId = (int)TaskItemStatus.InProgress;
        return await _context.Tasks.CountAsync(t => t.StatusId == inProgressId);
    }
}