using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskMasterPro.WPF.Models;

namespace TaskMasterPro.WPF.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5194/api/tasks";
        private readonly string _logFile = "task_service.log";

        public TaskService()
        {
            _httpClient = new HttpClient();
        }

        private void LogMessage(string message)
        {
            try
            {
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(_logFile, logEntry + Environment.NewLine);
            }
            catch
            {
                // Игнорируем ошибки логирования
            }
        }

        public async Task<List<TaskItem>> GetAllTasksAsync()
{
    try
    {
        LogMessage($"GET {_baseUrl}");
        var response = await _httpClient.GetAsync(_baseUrl);
        LogMessage($"Response status: {response.StatusCode}");
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        LogMessage($"Response content: {content}");
        
        // Десериализуем в List<JsonElement> сначала
        var tasksData = JsonSerializer.Deserialize<List<JsonElement>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        var tasks = new List<TaskItem>();
        foreach (var taskData in tasksData ?? new List<JsonElement>())
        {
            try
            {
                var task = new TaskItem
                {
                    Id = taskData.GetProperty("id").GetInt32(),
                    Title = taskData.GetProperty("title").GetString() ?? "",
                    Description = taskData.GetProperty("description").GetString() ?? "",
                    CreatedDate = DateTime.Parse(taskData.GetProperty("createdAt").GetString() ?? DateTime.Now.ToString()),
                    DueDate = taskData.TryGetProperty("dueDate", out var dueDate) && dueDate.ValueKind != JsonValueKind.Null 
                        ? DateTime.Parse(dueDate.GetString() ?? "") 
                        : null,
                    Priority = ParsePriority(taskData.GetProperty("priority").GetString() ?? "Medium"),
                    Status = ParseStatus(taskData.GetProperty("status").GetString() ?? "Pending"),
                    IsCompleted = taskData.GetProperty("status").GetString() == "Completed"
                };
                tasks.Add(task);
            }
            catch (Exception ex)
            {
                LogMessage($"Error parsing task: {ex.Message}");
            }
        }
        
        LogMessage($"Successfully parsed {tasks.Count} tasks");
        return tasks;
    }
    catch (Exception ex)
    {
        LogMessage($"Error getting tasks: {ex.Message}");
        LogMessage($"Full error: {ex}");
        return new List<TaskItem>();
    }
}

private TaskPriority ParsePriority(string priority)
{
    return priority.ToLower() switch
    {
        "low" => TaskPriority.Low,
        "medium" => TaskPriority.Medium,
        "high" => TaskPriority.High,
        "urgent" => TaskPriority.Critical,
        _ => TaskPriority.Medium
    };
}

private TaskItemStatus ParseStatus(string status)
{
    return status.ToLower() switch
    {
        "pending" => TaskItemStatus.ToDo,
        "inprogress" => TaskItemStatus.InProgress,
        "completed" => TaskItemStatus.Completed,
        "cancelled" => TaskItemStatus.Cancelled,
        _ => TaskItemStatus.ToDo
    };
}

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                LogMessage($"GET {url}");
                var response = await _httpClient.GetAsync(url);
                LogMessage($"Response status: {response.StatusCode}");
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                LogMessage($"Response content: {content}");
                
                return JsonSerializer.Deserialize<TaskItem>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                LogMessage($"Error getting task {id}: {ex.Message}");
                LogMessage($"Full error: {ex}");
                return null;
            }
        }

        public async Task<bool> CreateTaskAsync(TaskItem task)
        {
            try
            {
                // Отправляем только те поля, которые ожидает API
                var createTaskDto = new
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    EstimatedHours = (int?)null
                };
        
                var json = JsonSerializer.Serialize(createTaskDto);
                LogMessage($"POST {_baseUrl}");
                LogMessage($"JSON to send: {json}");
        
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseUrl, content);
        
                LogMessage($"Response status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    LogMessage($"Error response: {errorContent}");
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    LogMessage($"Success response: {responseContent}");
                }
        
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogMessage($"Error creating task: {ex.Message}");
                LogMessage($"Full error: {ex}");
                return false;
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskItem task)
        {
            try
            {
                // Создаем DTO для обновления
                var updateTaskDto = new
                {
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    EstimatedHours = (int?)null,
                    ActualHours = (int?)null
                };

                var json = JsonSerializer.Serialize(updateTaskDto);
                var url = $"{_baseUrl}/{task.Id}";
                LogMessage($"PUT {url}");
                LogMessage($"JSON to send: {json}");
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                
                LogMessage($"Response status: {response.StatusCode}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogMessage($"Error updating task: {ex.Message}");
                LogMessage($"Full error: {ex}");
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                LogMessage($"DELETE {url}");
                
                var response = await _httpClient.DeleteAsync(url);
                LogMessage($"Response status: {response.StatusCode}");
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LogMessage($"Error deleting task {id}: {ex.Message}");
                LogMessage($"Full error: {ex}");
                return false;
            }
        }
    }
} 