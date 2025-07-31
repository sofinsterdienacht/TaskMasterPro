


namespace TaskMasterPro.Core.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskItemStatus Status { get; set; }
    public TaskCategory Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
    
    public TaskItem()
    {
        CreatedAt = DateTime.UtcNow;
        Status = TaskItemStatus.Pending;
        Priority = TaskPriority.Medium;
        Category = TaskCategory.Personal;
    }
    
    public TaskItem(string title) : this()
    {
        Title = title;
    }
}