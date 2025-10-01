using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using TaskMasterPro.WPF.ViewModels;
using TaskMasterPro.WPF.Models;
using TaskMasterPro.WPF.Dialogs;

namespace TaskMasterPro.WPF
{






    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                DataContext = new MainViewModel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации окна: {ex.Message}", "Критическая ошибка");
            }
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Кнопка работает!", "Тест", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }


        private void TaskCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is TaskItem task && DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedTask = task;
            }
        }


        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task)
            {
                EditTaskDialog(task);
            }
            e.Handled = true; // Предотвращаем всплытие события
        }


        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task && DataContext is MainViewModel viewModel)
            {
                DeleteTaskDialog(task, viewModel);
            }
            e.Handled = true; // Предотвращаем всплытие события
        }


        private void EditSelectedTask_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.SelectedTask != null)
            {
                EditTaskDialog(viewModel.SelectedTask);
            }
        }


        private void DeleteSelectedTask_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.SelectedTask != null)
            {
                DeleteTaskDialog(viewModel.SelectedTask, viewModel);
            }
        }


        private void EditTaskDialog(TaskItem task)
        {
            var newTitle = Microsoft.VisualBasic.Interaction.InputBox(
                "Введите новое название задачи:",
                "Редактирование задачи",
                task.Title);
                
            if (!string.IsNullOrWhiteSpace(newTitle) && newTitle != task.Title)
            {
                var newDescription = Microsoft.VisualBasic.Interaction.InputBox(
                    "Введите новое описание задачи:",
                    "Редактирование задачи",
                    task.Description);


                task.Title = newTitle;
                if (!string.IsNullOrWhiteSpace(newDescription))
                {
                    task.Description = newDescription;
                }


                _ = UpdateTaskAsync(task);
            }
        }


        private void DeleteTaskDialog(TaskItem task, MainViewModel viewModel)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить задачу '{task.Title}'?", 
                "Подтверждение удаления", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                _ = DeleteTaskAsync(task, viewModel);
            }
        }


        private async Task UpdateTaskAsync(TaskItem task)
        {
            try
            {
                var configurationService = new TaskMasterPro.WPF.Services.ConfigurationService();
                var taskService = new TaskMasterPro.WPF.Services.TaskService(configurationService);
                var success = await taskService.UpdateTaskAsync(task);
                
                if (success)
                {
                    MessageBox.Show("Задача успешно обновлена!", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);


                    if (DataContext is MainViewModel viewModel)
                    {
                        await RefreshTasks(viewModel);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось обновить задачу.", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task DeleteTaskAsync(TaskItem task, MainViewModel viewModel)
        {
            try
            {
                var configurationService = new TaskMasterPro.WPF.Services.ConfigurationService();
                var taskService = new TaskMasterPro.WPF.Services.TaskService(configurationService);
                var success = await taskService.DeleteTaskAsync(task.Id);
                
                if (success)
                {


                    if (viewModel.SelectedTask?.Id == task.Id)
                    {
                        viewModel.SelectedTask = null;
                    }


                    await RefreshTasks(viewModel);
                }
                else
                {
                    MessageBox.Show("Не удалось удалить задачу.", "Ошибка", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async Task RefreshTasks(MainViewModel viewModel)
        {
            try
            {


                if (viewModel.LoadTasksCommand.CanExecute(null))
                {
                    viewModel.LoadTasksCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private async void CompleteSelectedTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DataContext is MainViewModel viewModel && viewModel.SelectedTask != null)
                {
                    var configurationService = new TaskMasterPro.WPF.Services.ConfigurationService();
                    var taskService = new TaskMasterPro.WPF.Services.TaskService(configurationService);
                    var success = await taskService.CompleteTaskAsync(viewModel.SelectedTask.Id);
                    if (success)
                    {
                        await RefreshTasks(viewModel);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось завершить задачу.", "Ошибка", 
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при завершении задачи: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}