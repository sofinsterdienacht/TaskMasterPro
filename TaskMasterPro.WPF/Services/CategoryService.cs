using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TaskMasterPro.WPF.Models;

namespace TaskMasterPro.WPF.Services
{
    public class CategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        

        public CategoryService(ConfigurationService configurationService)
        {
            var apiSettings = configurationService.GetApiSettings();
            var loggingSettings = configurationService.GetLoggingSettings();
            
            _baseUrl = $"{apiSettings.BaseUrl}{apiSettings.CategoriesEndpoint}";
            
            _httpClient = new HttpClient();
        }

        

        /// <summary>
        /// Получает все доступные категории задач
        /// </summary>
        public async Task<Dictionary<string, string[]>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_baseUrl);
                
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                var categoriesData = JsonSerializer.Deserialize<Dictionary<string, CategoryData>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                var result = new Dictionary<string, string[]>();
                foreach (var category in categoriesData ?? new Dictionary<string, CategoryData>())
                {
                    result[category.Key] = category.Value.Keywords;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                
                return new Dictionary<string, string[]>();
            }
        }

        /// <summary>
        /// Получает все доступные приоритеты задач
        /// </summary>
        public async Task<Dictionary<string, string[]>> GetAllPrioritiesAsync()
        {
            try
            {
                var url = $"{_baseUrl}/priorities";
                var response = await _httpClient.GetAsync(url);
                
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                var prioritiesData = JsonSerializer.Deserialize<Dictionary<string, CategoryData>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                var result = new Dictionary<string, string[]>();
                foreach (var priority in prioritiesData ?? new Dictionary<string, CategoryData>())
                {
                    result[priority.Key] = priority.Value.Keywords;
                }
                
                return result;
            }
            catch (Exception ex)
            {
                
                return new Dictionary<string, string[]>();
            }
        }

        /// <summary>
        /// Получает все значения перечислений (категории, приоритеты, статусы)
        /// </summary>
        public async Task<EnumValues> GetAllEnumValuesAsync()
        {
            try
            {
                var url = $"{_baseUrl}/all-enum-values";
                var response = await _httpClient.GetAsync(url);
                
                
                response.EnsureSuccessStatusCode();
                
                var content = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<EnumValues>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new EnumValues();
            }
            catch (Exception ex)
            {
                
                return new EnumValues();
            }
        }
    }

    public class CategoryData
    {
        public string[] Keywords { get; set; } = Array.Empty<string>();
    }

    public class EnumValues
    {
        public List<string> Priorities { get; set; } = new List<string>();
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Statuses { get; set; } = new List<string>();
    }
}