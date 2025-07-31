

namespace TaskMasterPro.Core.Entities;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum TaskItemStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}

public enum TaskCategory
{
    Personal = 1,
    Work = 2,
    Study = 3,
    Health = 4,
    Finance = 5,
    Shopping = 6
}