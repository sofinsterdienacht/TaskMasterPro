using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TaskMasterPro.WPF.Models;

namespace TaskMasterPro.WPF.Converters
{
    public class PriorityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TaskPriority priority)
            {
                return priority switch
                {
                    TaskPriority.Low => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Green
                    TaskPriority.Medium => new SolidColorBrush(Color.FromRgb(255, 152, 0)), // Orange
                    TaskPriority.High => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Red
                    TaskPriority.Critical => new SolidColorBrush(Color.FromRgb(156, 39, 176)), // Purple
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // Gray
                };
            }
            
            return new SolidColorBrush(Color.FromRgb(158, 158, 158)); // Default gray
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
//buукуку
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Visibility.Collapsed;
            
            bool invert = parameter?.ToString()?.ToLower() == "invert";
            bool boolValue = value is bool b ? b : value != null;
            
            if (invert) boolValue = !boolValue;
            
            return boolValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}