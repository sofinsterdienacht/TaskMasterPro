using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using TaskMasterPro.WPF.Models;

namespace TaskMasterPro.WPF.Services
{
    public class ConfigurationService
    {
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
            _appSettings = new AppSettings();
            _configuration.Bind(_appSettings);
        }

        public AppSettings GetAppSettings() => _appSettings;
        public ApiSettings GetApiSettings() => _appSettings.ApiSettings;
        public LoggingSettings GetLoggingSettings() => _appSettings.LoggingSettings;
    }
}
