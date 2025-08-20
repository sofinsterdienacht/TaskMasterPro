using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace TaskMasterPro.WPF.Dialogs
{
    public partial class CategorySelectDialog : Window
    {
        public string? SelectedCategory { get; private set; }

        public CategorySelectDialog(IEnumerable<string> categories, string? preselect = null)
        {
            InitializeComponent();
            CategoryCombo.ItemsSource = categories.ToList();
            if (!string.IsNullOrWhiteSpace(preselect) && categories.Contains(preselect))
            {
                CategoryCombo.SelectedItem = preselect;
            }
            else
            {
                CategoryCombo.SelectedIndex = 0;
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedCategory = CategoryCombo.SelectedItem as string;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}


