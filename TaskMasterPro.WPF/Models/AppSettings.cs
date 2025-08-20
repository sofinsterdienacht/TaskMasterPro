namespace TaskMasterPro.WPF.Models
{
    public class AppSettings
    {
        public ApiSettings ApiSettings { get; set; } = new();
        public LoggingSettings LoggingSettings { get; set; } = new();
    }

    public class ApiSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string TasksEndpoint { get; set; } = string.Empty;
        public string CategoriesEndpoint { get; set; } = string.Empty;
    }

    public class LoggingSettings
    {
        public string TaskServiceLogFile { get; set; } = string.Empty;
        public string CategoryServiceLogFile { get; set; } = string.Empty;
        public string MainViewModelLogFile { get; set; } = string.Empty;
        public string DebugLogFile { get; set; } = string.Empty;
    }
}

