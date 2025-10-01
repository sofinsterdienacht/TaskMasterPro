using System;
using System.Configuration;
using System.Data;
using System.Windows;

namespace TaskMasterPro.WPF;


/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка запуска приложения: {ex.Message}", "Критическая ошибка");
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}

