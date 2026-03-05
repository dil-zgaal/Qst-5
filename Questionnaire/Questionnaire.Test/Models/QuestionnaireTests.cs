using Questionnaire.Models;

namespace Questionnaire.Test.Models;

public class QuestionnaireTests
{
    [Fact]
    public void Questionnaire_CanBeCreated()
    {
        var id = TypedId<Questionnaire.Models.Questionnaire>.New();
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = id,
            Title = "Test Questionnaire",
            Description = "Test Description",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        Assert.Equal(id, questionnaire.Id);
        Assert.Equal("Test Questionnaire", questionnaire.Title);
        Assert.Equal("Test Description", questionnaire.Description);
        Assert.True(questionnaire.IsActive);
        Assert.Empty(questionnaire.Content);
    }

    [Fact]
    public void Questionnaire_CanHaveQuestions()
    {
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Test",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Content = new List<Question>
            {
                new TextQuestion
                {
                    Id = TypedId<Question>.New(),
                    Title = "What is your name?"
                }
            }
        };

        Assert.Single(questionnaire.Content);
        Assert.IsType<TextQuestion>(questionnaire.Content[0]);
        Assert.Equal("What is your name?", questionnaire.Content[0].Title);
    }

    [Fact]
    public void Question_CanHaveSubQuestions()
    {
        var question = new TextQuestion
        {
            Id = TypedId<Question>.New(),
            Title = "Main Question",
            SubQuestions = new List<Question>
            {
                new NumberQuestion
                {
                    Id = TypedId<Question>.New(),
                    Title = "Sub Question"
                }
            }
        };

        Assert.Single(question.SubQuestions);
        Assert.IsType<NumberQuestion>(question.SubQuestions[0]);
    }
}
