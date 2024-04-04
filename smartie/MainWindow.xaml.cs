using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Smartie
{
    public partial class MainWindow : Window
    {
        private DatabaseManager _dbManager;
        private List<QuizQuestion> _currentQuestions;
        private int _currentQuestionIndex = 0;

        public MainWindow()
        {
            InitializeComponent();
            _dbManager = new DatabaseManager();
            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = _dbManager.GetCategories();

            // Update the ComboBox for quiz category selection
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;

            // Update the ComboBox for adding new questions
            CategorySelectionForNewQuestion.ItemsSource = categories;
            CategorySelectionForNewQuestion.SelectedIndex = -1; // You might want to leave this unselected or select the first item as you prefer
        }


        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedCategory = (QuizCategory)CategoryComboBox.SelectedItem;
            if (selectedCategory != null)
            {
                _currentQuestions = _dbManager.GetQuestions(selectedCategory.Id, "Easy"); // Assuming "Easy" is a difficulty level
                DisplayQuestion(0);
            }
        }

        private void DisplayQuestion(int index)
        {
            if (index >= 0 && index < _currentQuestions.Count)
            {
                var question = _currentQuestions[index];
                QuestionTextBlock.Text = question.QuestionText;

                // Since Options is already a List<string>, we can directly use it
                OptionsListBox.ItemsSource = question.Options;

                OptionsListBox.SelectedIndex = -1; // Reset selection
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentQuestionIndex > 0)
            {
                DisplayQuestion(--_currentQuestionIndex);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsListBox.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an option.");
                return;
            }

            var selectedQuestion = _currentQuestions[_currentQuestionIndex];
            bool isCorrect = OptionsListBox.SelectedIndex == selectedQuestion.CorrectAnswerIndex;
            MessageBox.Show(isCorrect ? "Correct!" : "Incorrect.", "Result");

            if (_currentQuestionIndex < _currentQuestions.Count - 1)
            {
                DisplayQuestion(++_currentQuestionIndex);
            }
            else
            {
                MessageBox.Show("You have completed the quiz!", "Finished");
                // Additional logic for what happens when the quiz is finished
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = NewCategoryName.Text.Trim();
            if (!string.IsNullOrEmpty(categoryName))
            {
                _dbManager.InsertCategory(categoryName);
                LoadCategories(); // Refresh categories after insertion
                NewCategoryName.Clear();

                // Additional logic might be needed here if you want to select the new category
                // in the CategorySelectionForNewQuestion ComboBox or display a success message.
            }
            else
            {
                MessageBox.Show("Please enter a category name.");
            }
        }

        private void AddQuestion_Click(object sender, RoutedEventArgs e)
        {
            // Simple validation
            if (CategorySelectionForNewQuestion.SelectedItem == null ||
                string.IsNullOrWhiteSpace(NewQuestionText.Text) ||
                string.IsNullOrWhiteSpace(NewQuestionOptions.Text) ||
                !int.TryParse(NewQuestionAnswerIndex.Text, out int correctAnswerIndex))
            {
                MessageBox.Show("Please fill in all question fields correctly.");
                return;
            }

            var newQuestion = new QuizQuestion
            {
                CategoryId = (int)CategorySelectionForNewQuestion.SelectedValue,
                QuestionText = NewQuestionText.Text.Trim(),
                Options = NewQuestionOptions.Text.Split('|').Select(o => o.Trim()).ToList(),
                CorrectAnswerIndex = correctAnswerIndex,
                Explanation = NewQuestionExplanation.Text.Trim()
            };

            _dbManager.InsertQuestion(newQuestion);
            // Optionally clear the input fields
        }
    }
}
