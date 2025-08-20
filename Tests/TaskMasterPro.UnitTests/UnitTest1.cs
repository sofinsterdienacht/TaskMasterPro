using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Services;
using Xunit;

namespace TaskMasterPro.UnitTests;

public class CategoryServiceTests
{
    [Theory]
    [InlineData("Срочно сделать отчёт по проекту", TaskPriority.High)]
    [InlineData("Urgent meeting", TaskPriority.High)]
    [InlineData("Запланировать бюджет", TaskPriority.Medium)]
    public void DeterminePriority_ByKeywords_ReturnsExpected(string title, TaskPriority expected)
    {
        var service = new CategoryService();
        var result = service.DeterminePriority(title);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("конференция по клиенту", TaskCategory.Work)]
    [InlineData("купить продукты", TaskCategory.Shopping)]
    [InlineData("сходить к врачу", TaskCategory.Health)]
    public void DetermineCategory_ByKeywords_ReturnsExpected(string title, TaskCategory expected)
    {
        var service = new CategoryService();
        var result = service.DetermineCategory(title);
        Assert.Equal(expected, result);
    }
}