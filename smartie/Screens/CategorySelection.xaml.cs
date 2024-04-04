using System;
using System.Windows;
using System.Windows.Controls;

namespace Smartie.Screens
{
    // It's a good practice to define the EventArgs class outside of the UserControl class, but it can stay here if it's only used by this UserControl.
    public class CategorySelectedEventArgs : EventArgs
    {
        public QuizCategory SelectedCategory { get; }

        public CategorySelectedEventArgs(QuizCategory selectedCategory)
        {
            SelectedCategory = selectedCategory;
        }
    }

    public partial class CategorySelection : UserControl
    {
        public event EventHandler<CategorySelectedEventArgs> CategorySelected;

        private DatabaseManager _dbManager;

        public CategorySelection()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager(); // Adjusted to directly include the connection string
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                var categories = _dbManager.GetCategories();
                // Ensure CategoriesListBox is correctly named in the XAML file.
                CategoriesListBox.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        // This method should be wired up to the SelectionChanged event of CategoriesListBox in the XAML.
        private void CategoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var selectedCategory = listBox?.SelectedItem as QuizCategory;
            if (selectedCategory != null)
            {
                // Invoking the event to notify subscribers (potentially the MainWindow) about the category selection.
                CategorySelected?.Invoke(this, new CategorySelectedEventArgs(selectedCategory));
            }
        }
    }
}