using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using TaskMasterPro.WPF.ViewModels;
using TaskMasterPro.WPF.Models;

namespace TaskMasterPro.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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

        // Обработчик прокрутки мышкой
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        // Обработчик клика по карточке задачи для выбора
        private void TaskCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is TaskItem task && DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedTask = task;
            }
        }

        // Обработчик кнопки редактирования на карточке
        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task)
            {
                EditTaskDialog(task);
            }
            e.Handled = true; // Предотвращаем всплытие события
        }

        // Обработчик кнопки удаления на карточке
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TaskItem task && DataContext is MainViewModel viewModel)
            {
                DeleteTaskDialog(task, viewModel);
            }
            e.Handled = true; // Предотвращаем всплытие события
        }

        // Обработчик кнопки редактирования в панели деталей
        private void EditSelectedTask_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.SelectedTask != null)
            {
                EditTaskDialog(viewModel.SelectedTask);
            }
        }

        // Обработчик кнопки удаления в панели деталей
        private void DeleteSelectedTask_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel && viewModel.SelectedTask != null)
            {
                DeleteTaskDialog(viewModel.SelectedTask, viewModel);
            }
        }

        // Диалог редактирования задачи
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
                    
                // Обновляем задачу
                task.Title = newTitle;
                if (!string.IsNullOrWhiteSpace(newDescription))
                {
                    task.Description = newDescription;
                }
                
                // Вызываем обновление через API
                _ = UpdateTaskAsync(task);
            }
        }

        // Диалог удаления задачи
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

        // Метод обновления задачи
        private async Task UpdateTaskAsync(TaskItem task)
        {
            try
            {
                var taskService = new TaskMasterPro.WPF.Services.TaskService();
                var success = await taskService.UpdateTaskAsync(task);
                
                if (success)
                {
                    MessageBox.Show("Задача успешно обновлена!", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    // Обновляем список задач
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

        // Метод удаления задачи
        private async Task DeleteTaskAsync(TaskItem task, MainViewModel viewModel)
        {
            try
            {
                var taskService = new TaskMasterPro.WPF.Services.TaskService();
                var success = await taskService.DeleteTaskAsync(task.Id);
                
                if (success)
                {
                    MessageBox.Show("Задача успешно удалена!", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    // Сбрасываем выбранную задачу если она была удалена
                    if (viewModel.SelectedTask?.Id == task.Id)
                    {
                        viewModel.SelectedTask = null;
                    }
                    // Обновляем список задач
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

        // Метод обновления списка задач
        private async Task RefreshTasks(MainViewModel viewModel)
        {
            try
            {
                // Имитируем выполнение команды загрузки
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
    }
}