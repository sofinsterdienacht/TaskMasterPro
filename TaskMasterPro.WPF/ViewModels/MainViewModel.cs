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

namespace TaskMasterPro.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly TaskService _taskService;
        private TaskItem? _selectedTask;
        private bool _isLoading;
        private readonly string _logFile = "main_viewmodel.log";
        private readonly string _debugLogFile = Path.Combine("logs", "wpf_debug.log");

        public MainViewModel()
        {
            _taskService = new TaskService();
            Tasks = new ObservableCollection<TaskItem>();
            
            LoadTasksCommand = new RelayCommand(async () => await LoadTasksAsync());
            AddTaskCommand = new RelayCommand(async () => await AddTaskAsync());
            DeleteTaskCommand = new RelayCommand(async () => await DeleteTaskAsync());
            
            // Загружаем задачи при запуске
            _ = LoadTasksAsync();
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

        private void DebugLog(string message)
        {
            try
            {
                Directory.CreateDirectory("logs");
                var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                File.AppendAllText(_debugLogFile, logEntry + Environment.NewLine);
            }
            catch { }
        }

        public ObservableCollection<TaskItem> Tasks { get; }

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

        private async Task LoadTasksAsync()
        {
            LogMessage("LoadTasksAsync started");
            IsLoading = true;
            try
            {
                var tasks = await _taskService.GetAllTasksAsync();
                LogMessage($"Loaded {tasks.Count} tasks");
                Tasks.Clear();
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Error in LoadTasksAsync: {ex.Message}");
                LogMessage($"Full error: {ex}");
                MessageBox.Show($"Ошибка загрузки задач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
                LogMessage("LoadTasksAsync completed");
            }
        }

        private async Task AddTaskAsync()
        {
            DebugLog("AddTaskAsync вызван");
            try
            {
                DebugLog("Показываю InputBox для названия");
                var taskTitle = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите название задачи:",
                    "Новая задача",
                    "Новая задача");
                DebugLog($"User entered title: {taskTitle}");
                if (string.IsNullOrWhiteSpace(taskTitle))
                {
                    DebugLog("User cancelled or entered empty title");
                    return;
                }
                DebugLog("Показываю InputBox для описания");
                var taskDescription = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите описание задачи:",
                    "Описание задачи",
                    "Описание задачи");
                DebugLog($"User entered description: {taskDescription}");
                var newTask = new TaskItem
                {
                    Title = taskTitle,
                    Description = string.IsNullOrEmpty(taskDescription) ? "Описание задачи" : taskDescription,
                    CreatedDate = DateTime.Now,
                    Priority = TaskPriority.Medium,
                    Status = TaskItemStatus.ToDo,
                    IsCompleted = false
                };
                DebugLog($"Создан TaskItem: {newTask.Title}");
                bool success = false;
                try
                {
                    DebugLog("Пробую вызвать _taskService.CreateTaskAsync");
                    success = await _taskService.CreateTaskAsync(newTask);
                    DebugLog($"CreateTaskAsync вернул: {success}");
                }
                catch (Exception ex)
                {
                    DebugLog($"Exception в CreateTaskAsync: {ex.Message}");
                }
                MessageBox.Show(success ? "Задача успешно создана!" : "Не удалось создать задачу.");
                if (success)
                {
                    DebugLog("Задача успешно создана, вызываю LoadTasksAsync");
                    await LoadTasksAsync();
                    MessageBox.Show($"Загружено задач: {Tasks.Count}");
                }
                else
                {
                    DebugLog("Не удалось создать задачу");
                }
            }
            catch (Exception ex)
            {
                DebugLog($"Exception в AddTaskAsync: {ex.Message}");
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
                    LogMessage($"DeleteTaskAsync for task {SelectedTask.Id}");
                    var success = await _taskService.DeleteTaskAsync(SelectedTask.Id);
                    LogMessage($"Delete task result: {success}");
                    if (success)
                    {
                        await LoadTasksAsync();
                        SelectedTask = null;
                        MessageBox.Show("Задача успешно удалена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить задачу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Простая реализация ICommand
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