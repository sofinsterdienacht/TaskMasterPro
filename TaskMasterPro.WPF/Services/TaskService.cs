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
        private readonly string _baseUrl;
        

        public TaskService(ConfigurationService configurationService)
        {
            var apiSettings = configurationService.GetApiSettings();
            var loggingSettings = configurationService.GetLoggingSettings();
            
            _baseUrl = $"{apiSettings.BaseUrl}{apiSettings.TasksEndpoint}";
            
            _httpClient = new HttpClient();
        }

        

        public async Task<List<TaskItem>> GetAllTasksAsync()
{
    try
    {
        var response = await _httpClient.GetAsync(_baseUrl);
        
        
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        
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
                    Category = ParseCategory(taskData.GetProperty("category").GetString() ?? "Personal"),
                    IsCompleted = taskData.GetProperty("status").GetString() == "Completed"
                };
                tasks.Add(task);
            }
            catch (Exception ex)
            {
                
            }
        }
        
        return tasks;
    }
    catch (Exception ex)
    {
        
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

private TaskCategory ParseCategory(string category)
{
    return category.ToLower() switch
    {
        "personal" => TaskCategory.Personal,
        "work" => TaskCategory.Work,
        "study" => TaskCategory.Study,
        "health" => TaskCategory.Health,
        "finance" => TaskCategory.Finance,
        "shopping" => TaskCategory.Shopping,
        _ => TaskCategory.Personal
    };
}

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                var response = await _httpClient.GetAsync(url);
                
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<TaskItem>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                
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
                
        
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(_baseUrl, content);
        
        
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
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
                
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}";
                var response = await _httpClient.DeleteAsync(url);
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public async Task<bool> CompleteTaskAsync(int id)
        {
            try
            {
                var url = $"{_baseUrl}/{id}/complete";
                var response = await _httpClient.PostAsync(url, new StringContent("{}", Encoding.UTF8, "application/json"));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
    }
} 