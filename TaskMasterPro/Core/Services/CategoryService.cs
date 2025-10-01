using TaskMasterPro.Core.Entities;
using TaskMasterPro.Core.Interfaces;
using System.Linq;

namespace TaskMasterPro.Core.Services;

public class CategoryService : ICategoryService
{


    private readonly Dictionary<TaskCategory, string[]> _categoryKeywords = new()
    {
        { TaskCategory.Work, new[] { "работ", "work", "проект", "клиент", "презентац", "presentation", 
                                    "отчет", "report", "совещан", "meeting", "конференц", "conference" } },
        { TaskCategory.Study, new[] { "учеб", "study", "экзамен", "курс" } },
        { TaskCategory.Health, new[] { "здоров", "health", "врач", "спорт" } },
        { TaskCategory.Finance, new[] { "денег", "finance", "бюджет", "счет" } },
        { TaskCategory.Shopping, new[] { "покуп", "shopping", "купи", "магазин" } },
        { TaskCategory.Personal, new[] { "личн", "personal", "дом", "семья" } }
    };


    private readonly Dictionary<TaskPriority, string[]> _priorityKeywords = new()
    {
        { TaskPriority.High, new[] { "срочно", "urgent", "критично", "важно" } },
        { TaskPriority.Medium, new[] { "средний", "medium", "нормально" } },
        { TaskPriority.Low, new[] { "не важно", "потом", "low" } },
        { TaskPriority.Urgent, new[] { "критично", "urgent", "немедленно" } }
    };


    private readonly string[] _allCategoryKeywords;
    private readonly string[] _allPriorityKeywords;


    private readonly Dictionary<string, TaskCategory> _keywordToCategory;
    private readonly Dictionary<string, TaskPriority> _keywordToPriority;

    public CategoryService()
    {


        _keywordToCategory = new Dictionary<string, TaskCategory>();
        foreach (var pair in _categoryKeywords)
        {
            foreach (var keyword in pair.Value)
            {


                _keywordToCategory[keyword] = pair.Key;
            }
        }
        _allCategoryKeywords = _keywordToCategory.Keys.Distinct().ToArray();

        _keywordToPriority = new Dictionary<string, TaskPriority>();
        foreach (var pair in _priorityKeywords)
        {
            foreach (var keyword in pair.Value)
            {




                if (!_keywordToPriority.ContainsKey(keyword))
                {
                    _keywordToPriority[keyword] = pair.Key;
                }
            }
        }
        _allPriorityKeywords = _keywordToPriority.Keys.Distinct().ToArray();
    }








    public Dictionary<TaskCategory, string[]> GetAllCategories() => _categoryKeywords;








    public Dictionary<TaskPriority, string[]> GetAllPriorities() => _priorityKeywords;










    public TaskCategory DetermineCategory(string title)
    {
        var lowerTitle = title.ToLower();


        foreach (var keyword in _allCategoryKeywords)
        {
            if (lowerTitle.Contains(keyword))
            {


                return _keywordToCategory[keyword];
            }
        }


        return TaskCategory.Personal;
    }










    public TaskPriority DeterminePriority(string title)
    {
        var lowerTitle = title.ToLower();


        foreach (var keyword in _allPriorityKeywords)
        {
            if (lowerTitle.Contains(keyword))
            {
                return _keywordToPriority[keyword];
            }
        }


        return TaskPriority.Medium;
    }
}