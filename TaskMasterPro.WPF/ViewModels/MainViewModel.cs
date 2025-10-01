using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TaskMasterPro.WPF.Models;
using TaskMasterPro.WPF.Services;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using TaskMasterPro.WPF.Dialogs;

namespace TaskMasterPro.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private readonly CategoryService _categoryService;
        private readonly ConfigurationService _configurationService;
        private TaskItem? _selectedTask;
        private bool _isLoading;
        
        
        private List<string> _availableCategories = new List<string>();
        private List<string> _availablePriorities = new List<string>();

        public MainViewModel()
        {
            _configurationService = new ConfigurationService();
            
            _taskService = new TaskService(_configurationService);
            _categoryService = new CategoryService(_configurationService);
            Tasks = new ObservableCollection<TaskItem>();
            
            LoadTasksCommand = new RelayCommand(async () => await LoadTasksAsync());
            AddTaskCommand = new RelayCommand(async () => await AddTaskAsync());
            DeleteTaskCommand = new RelayCommand(async () => await DeleteTaskAsync());
            StatisticsCommand = new RelayCommand(async () => await ShowStatisticsAsync());


            _ = InitializeAsync();
        }

        

        public ObservableCollection<TaskItem> Tasks { get; }
        
        public List<string> AvailableCategories
        {
            get => _availableCategories;
            private set
            {
                _availableCategories = value;
                OnPropertyChanged();
            }
        }
        
        public List<string> AvailablePriorities
        {
            get => _availablePriorities;
            private set
            {
                _availablePriorities = value;
                OnPropertyChanged();
            }
        }

        public TaskItem? SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadTasksCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand StatisticsCommand { get; }

        private async Task InitializeAsync()
        {
            
            IsLoading = true;
            try
            {


                await LoadCategoriesAndPrioritiesAsync();


                await LoadTasksAsync();
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                
            }
        }
        
        private async Task LoadCategoriesAndPrioritiesAsync()
        {
            try
            {
                var enumValues = await _categoryService.GetAllEnumValuesAsync();
                
                AvailableCategories = enumValues.Categories;
                AvailablePriorities = enumValues.Priorities;
                
            }
            catch (Exception ex)
            {


                AvailableCategories = Enum.GetNames(typeof(TaskCategory)).ToList();
                AvailablePriorities = Enum.GetNames(typeof(TaskPriority)).ToList();
            }
        }
        
        private async Task LoadTasksAsync()
        {
            
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка загрузки задач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task AddTaskAsync()
        {
            try
            {
                var taskTitle = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите название задачи:",
                    "Новая задача",
                    "Новая задача");
                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    return;
                }
                
                var taskDescription = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите описание задачи:",
                    "Описание задачи",
                    "Описание задачи");


                string selectedCategory;
                try
                {
                    var categoriesKeywords = await _categoryService.GetAllCategoriesAsync();
                    var lowerTitle = taskTitle.ToLower();
                    var inferred = categoriesKeywords
                        .Select(kvp => new { Name = kvp.Key, Match = kvp.Value.Any(k => lowerTitle.Contains(k)) })
                        .FirstOrDefault(x => x.Match)?.Name;

                    if (!string.IsNullOrWhiteSpace(inferred) && AvailableCategories.Contains(inferred))
                    {
                        selectedCategory = inferred;
                        
                    }
                    else
                    {


                        var dialog = new CategorySelectDialog(AvailableCategories);
                        var owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                        if (owner != null) dialog.Owner = owner;
                        var ok = dialog.ShowDialog() == true;
                        selectedCategory = ok && !string.IsNullOrWhiteSpace(dialog.SelectedCategory)
                            ? dialog.SelectedCategory!
                            : (AvailableCategories.FirstOrDefault() ?? "Personal");
                        
                    }
                }
                catch (Exception ex)
                {
                    
                    var dialog = new CategorySelectDialog(AvailableCategories);
                    var owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
                    if (owner != null) dialog.Owner = owner;
                    var ok = dialog.ShowDialog() == true;
                    selectedCategory = ok && !string.IsNullOrWhiteSpace(dialog.SelectedCategory)
                        ? dialog.SelectedCategory!
                        : (AvailableCategories.FirstOrDefault() ?? "Personal");
                }


                var selectedPriority = "Medium";
                
                
                var newTask = new TaskItem
                {
                    Title = taskTitle,
                    Description = string.IsNullOrEmpty(taskDescription) ? "Описание задачи" : taskDescription,
                    CreatedDate = DateTime.Now,
                    Priority = Enum.Parse<TaskPriority>(selectedPriority),
                    Category = Enum.Parse<TaskCategory>(selectedCategory),
                    Status = TaskItemStatus.ToDo,
                    IsCompleted = false
                };
                bool success = false;
                try
                {
                    success = await _taskService.CreateTaskAsync(newTask);
                    
                }
                catch (Exception ex)
                {
                    
                }
                if (success)
                {
                    await LoadTasksAsync();
                }
                
            }
            catch (Exception ex)
            {
                
            }
        }

        private async Task DeleteTaskAsync()
        {
            if (SelectedTask != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить задачу '{SelectedTask.Title}'?", 
                                           "Подтверждение удаления", 
                                           MessageBoxButton.YesNo, 
                                           MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    var success = await _taskService.DeleteTaskAsync(SelectedTask.Id);
                    if (success)
                    {
                        await LoadTasksAsync();
                        SelectedTask = null;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить задачу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async Task ShowStatisticsAsync()
        {
            try
            {
                var totalTasks = Tasks.Count;
                var completedTasks = Tasks.Count(t => t.IsCompleted);
                var pendingTasks = totalTasks - completedTasks;
                var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

                var categoryStats = Tasks.GroupBy(t => t.Category)
                    .Select(g => $"{g.Key}: {g.Count()} задач")
                    .ToList();

                var priorityStats = Tasks.GroupBy(t => t.Priority)
                    .Select(g => $"{g.Key}: {g.Count()} задач")
                    .ToList();

                var statsMessage = $"📊 СТАТИСТИКА ЗАДАЧ\n\n" +
                                 $"Всего задач: {totalTasks}\n" +
                                 $"Завершено: {completedTasks}\n" +
                                 $"В работе: {pendingTasks}\n" +
                                 $"Процент выполнения: {completionRate:F1}%\n\n" +
                                 $"📂 ПО КАТЕГОРИЯМ:\n{string.Join("\n", categoryStats)}\n\n" +
                                 $"⚡ ПО ПРИОРИТЕТАМ:\n{string.Join("\n", priorityStats)}";

                MessageBox.Show(statsMessage, "Статистика задач", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class RelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public async void Execute(object? parameter)
        {
            await _execute();
        }
    }
}