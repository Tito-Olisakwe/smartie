using System.Collections.Generic;

public class QuizQuestion
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Difficulty { get; set; }
    public string QuestionText { get; set; }
    public List<string> Options { get; set; }
    public int CorrectAnswerIndex { get; set; } 
    public string Explanation { get; set; }
}
