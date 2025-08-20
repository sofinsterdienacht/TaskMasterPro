using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskMasterPro.API.DTOs;
using Xunit;

namespace TaskMasterPro.IntegrationTests;

public class TasksApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TasksApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { });
    }

    [Fact]
    public async Task CreateTask_AutoSetsCategoryAndPriority()
    {
        var client = _factory.CreateClient();
        var dto = new CreateTaskDto
        {
            Title = "Срочно подготовить презентацию для конференции",
            Description = "",
            DueDate = null,
            EstimatedHours = null
        };

        var response = await client.PostAsJsonAsync("/api/tasks", dto);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TaskResponseDto>();
        Assert.NotNull(created);
        Assert.Equal("Work", created!.Category);
        Assert.True(created.Priority is "High" or "Urgent");
    }
}