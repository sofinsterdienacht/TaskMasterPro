using System;

namespace TaskMasterPro.WPF.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }
        public bool IsCompleted { get; set; }
    
        // Добавьте эти поля для соответствия API
        public DateTime? CompletedAt { get; set; }
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }
        public TaskCategory Category { get; set; }
    }

// Добавьте enum TaskCategory
    public enum TaskCategory
    {
        Personal,
        Work,
        Study,
        Health,
        Finance,
        Shopping
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum TaskItemStatus
    {
        ToDo,
        InProgress,
        Completed,
        Cancelled
    }
} 