


using System.ComponentModel.DataAnnotations.Schema;

namespace TaskMasterPro.Core.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }


    public int PriorityId { get; set; }
    public int StatusId { get; set; }
    public int CategoryId { get; set; }


    public PriorityLookup? PriorityRef { get; set; }
    public StatusLookup? StatusRef { get; set; }
    public CategoryLookup? CategoryRef { get; set; }


    [NotMapped]
    public TaskPriority Priority
    {
        get => (TaskPriority)PriorityId;
        set => PriorityId = (int)value;
    }

    [NotMapped]
    public TaskItemStatus Status
    {
        get => (TaskItemStatus)StatusId;
        set => StatusId = (int)value;
    }

    [NotMapped]
    public TaskCategory Category
    {
        get => (TaskCategory)CategoryId;
        set => CategoryId = (int)value;
    }

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