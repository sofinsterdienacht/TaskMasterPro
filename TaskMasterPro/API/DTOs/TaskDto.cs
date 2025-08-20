namespace TaskMasterPro.API.DTOs;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
}

public class UpdateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
}

public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string? DueDate { get; set; }
    public string? CompletedAt { get; set; }
    public int? EstimatedHours { get; set; }
    public int? ActualHours { get; set; }
}