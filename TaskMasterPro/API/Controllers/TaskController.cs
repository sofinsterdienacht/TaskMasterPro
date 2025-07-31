

using Microsoft.AspNetCore.Mvc;
using TaskMasterPro.API.DTOs;
using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;

namespace TaskMasterPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetAllTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        var response = tasks.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTaskById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound($"Задача с ID {id} не найдена");

        return Ok(MapToResponseDto(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto dto)
    {
        try
        {
            var task = new TaskItem()
            {
                Title = dto.Title,
                Description = dto.Description,
                DueDate = dto.DueDate,
                EstimatedHours = dto.EstimatedHours
            };

            var result = await _taskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, MapToResponseDto(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TaskResponseDto>> UpdateTask(int id, UpdateTaskDto dto)
    {
        try
        {
            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
                return NotFound($"Задача с ID {id} не найдена");

            existingTask.Title = dto.Title;
            existingTask.Description = dto.Description;
            existingTask.DueDate = dto.DueDate;
            existingTask.EstimatedHours = dto.EstimatedHours;
            existingTask.ActualHours = dto.ActualHours;

            var result = await _taskService.UpdateTaskAsync(existingTask);
            return Ok(MapToResponseDto(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteTask(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound($"Задача с ID {id} не найдена");

        await _taskService.DeleteTaskAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult<TaskResponseDto>> CompleteTask(int id)
    {
        try
        {
            var result = await _taskService.CompleteTaskAsync(id);
            return Ok(MapToResponseDto(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/start")]
    public async Task<ActionResult<TaskResponseDto>> StartTask(int id)
    {
        try
        {
            var result = await _taskService.StartTaskAsync(id);
            return Ok(MapToResponseDto(result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("priority/{priority}")]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByPriority(string priority)
    {
        if (!Enum.TryParse<TaskPriority>(priority, true, out var priorityEnum))
            return BadRequest($"Неверный приоритет: {priority}");

        var tasks = await _taskService.GetTasksByPriorityAsync(priorityEnum);
        var response = tasks.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("status/{status}")]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByStatus(string status)
    {
        if (!Enum.TryParse<TaskItemStatus>(status, true, out var statusEnum))
            return BadRequest($"Неверный статус: {status}");

        var tasks = await _taskService.GetTasksByStatusAsync(statusEnum);
        var response = tasks.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<TaskResponseDto>>> GetTasksByCategory(string category)
    {
        if (!Enum.TryParse<TaskCategory>(category, true, out var categoryEnum))
            return BadRequest($"Неверная категория: {category}");

        var tasks = await _taskService.GetTasksByCategoryAsync(categoryEnum);
        var response = tasks.Select(MapToResponseDto);
        return Ok(response);
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<object>> GetStatistics()
    {
        var statistics = await _taskService.GetStatisticsAsync();
        return Ok(statistics);
    }

    private static TaskResponseDto MapToResponseDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Priority = task.Priority.ToString(),
            Status = task.Status.ToString(),
            Category = task.Category.ToString(),
            CreatedAt = task.CreatedAt.ToString("yyyy-MM-dd"),
            DueDate = task.DueDate?.ToString("yyyy-MM-dd"),
            CompletedAt = task.CompletedAt?.ToString("yyyy-MM-dd"),
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours
        };
    }
}