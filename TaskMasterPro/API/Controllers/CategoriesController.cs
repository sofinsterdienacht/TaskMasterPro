using Microsoft.AspNetCore.Mvc;
using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;

namespace TaskMasterPro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("priorities")]
    public ActionResult<object> GetAllPriorities()
    {
        var priorities = _categoryService.GetAllPriorities();
        var result = priorities.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => new { Keywords = kvp.Value }
        );
        return Ok(result);
    }

    [HttpGet]
    public ActionResult<object> GetAllCategories()
    {
        var categories = _categoryService.GetAllCategories();
        var result = categories.ToDictionary(
            kvp => kvp.Key.ToString(),
            kvp => new { Keywords = kvp.Value }
        );
        return Ok(result);
    }

    [HttpGet("all-enum-values")]
    public ActionResult<object> GetAllEnumValues()
    {
        var priorities = Enum.GetValues(typeof(TaskPriority))
            .Cast<TaskPriority>()
            .Select(p => p.ToString())
            .ToList();

        var categories = Enum.GetValues(typeof(TaskCategory))
            .Cast<TaskCategory>()
            .Select(c => c.ToString())
            .ToList();

        var statuses = Enum.GetValues(typeof(TaskItemStatus))
            .Cast<TaskItemStatus>()
            .Select(s => s.ToString())
            .ToList();

        return Ok(new
        {
            Priorities = priorities,
            Categories = categories,
            Statuses = statuses
        });
    }
}