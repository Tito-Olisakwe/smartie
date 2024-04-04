using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

public class DatabaseManager
{
    private readonly string _connectionString;

    public DatabaseManager()
    {
        _connectionString = "Data Source=Database\\smartie.db;Version=3;";
    }

    public List<QuizCategory> GetCategories()
    {
        var categories = new List<QuizCategory>();

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name FROM Categories";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            categories.Add(new QuizCategory
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return categories;
    }

    public List<QuizQuestion> GetQuestions(int categoryId, string difficulty)
    {
        var questions = new List<QuizQuestion>();

        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, CategoryId, Difficulty, QuestionText, CorrectAnswerIndex, Options, Explanation FROM Questions WHERE CategoryId = @CategoryId AND Difficulty = @Difficulty";
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        command.Parameters.AddWithValue("@Difficulty", difficulty);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            // Check if the reader has the expected number of columns
            if(reader.FieldCount < 7)
            {
                // Handle the situation, maybe log an error or throw a custom exception
                throw new InvalidOperationException("Not enough columns in the result set.");
            }

            questions.Add(new QuizQuestion
            {
                Id = reader.GetInt32(0),
                CategoryId = reader.GetInt32(1),
                Difficulty = reader.GetString(2),
                QuestionText = reader.GetString(3),
                CorrectAnswerIndex = reader.GetInt32(4),
                Options = reader.GetString(5).Split('|').ToList(),
                Explanation = reader.FieldCount > 6 ? reader.GetString(6) : string.Empty // Safeguard in case the column is missing
            });
        }

        return questions;
    }

    public void InsertCategory(string name)
    {
        try
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Categories (Name) VALUES (@Name)";
            command.Parameters.AddWithValue("@Name", name);

            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            // Handle the exception
            // For example, log the error message and/or notify the user
            Console.WriteLine(ex.Message);
            // If you have logging implemented, log the exception here
        }
    }

    public void InsertQuestion(QuizQuestion question)
    {
        using var connection = new SQLiteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
        INSERT INTO Questions (CategoryId, Difficulty, QuestionText, CorrectAnswerIndex, Options, Explanation) 
        VALUES (@CategoryId, @Difficulty, @QuestionText, @CorrectAnswerIndex, @Options, @Explanation)";

        command.Parameters.AddWithValue("@CategoryId", question.CategoryId);
        command.Parameters.AddWithValue("@Difficulty", question.Difficulty);
        command.Parameters.AddWithValue("@QuestionText", question.QuestionText);
        command.Parameters.AddWithValue("@CorrectAnswerIndex", question.CorrectAnswerIndex);
        command.Parameters.AddWithValue("@Options", string.Join("|", question.Options));
        command.Parameters.AddWithValue("@Explanation", question.Explanation);

        command.ExecuteNonQuery();
    }

    public void TestDatabaseConnection()
    {
        try
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                connection.Open();
                Console.WriteLine("Successfully connected to the database.");
                // Optionally, execute a simple test query...
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Categories", connection))
                {
                    var count = (long)command.ExecuteScalar();
                    Console.WriteLine($"There are {count} categories in the database.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while connecting to the database: " + ex.Message);
        }
    }
}