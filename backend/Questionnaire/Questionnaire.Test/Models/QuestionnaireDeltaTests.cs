using Questionnaire.Contract.Models;

namespace Questionnaire.Test.Models;

public class QuestionnaireDeltaTests
{
    [Fact]
    public void Delta_CanUpdateTitle()
    {
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Original Title",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var delta = new QuestionnaireDelta
        {
            Title = Patchable<string>.Set("New Title")
        };

        delta.Apply(questionnaire);

        Assert.Equal("New Title", questionnaire.Title);
    }

    [Fact]
    public void Delta_CanClearDescription()
    {
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Title",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var delta = new QuestionnaireDelta
        {
            Description = Patchable<string?>.Clear()
        };

        delta.Apply(questionnaire);

        Assert.Null(questionnaire.Description);
    }

    [Fact]
    public void Delta_CanAddQuestionToContent()
    {
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Title",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var newQuestion = new TextQuestion
        {
            Id = TypedId<Question>.New(),
            Title = "New Question"
        };

        var delta = new QuestionnaireDelta
        {
            ContentPatch = ArrayPatch<Question>.Add(newQuestion)
        };

        delta.Apply(questionnaire);

        Assert.Single(questionnaire.Content);
        Assert.Equal(newQuestion.Id, questionnaire.Content[0].Id);
    }

    [Fact]
    public void Delta_CanRemoveQuestionById()
    {
        var questionId = TypedId<Question>.New();
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Title",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Content = new List<Question>
            {
                new TextQuestion { Id = questionId, Title = "Question to remove" },
                new NumberQuestion { Id = TypedId<Question>.New(), Title = "Other question" }
            }
        };

        var delta = new QuestionnaireDelta
        {
            ContentPatch = ArrayPatch<Question>.RemoveById(questionId)
        };

        delta.Apply(questionnaire);

        Assert.Single(questionnaire.Content);
        Assert.NotEqual(questionId, questionnaire.Content[0].Id);
    }

    [Fact]
    public void Delta_CanUpdateQuestionTitle()
    {
        var questionId = TypedId<Question>.New();
        var questionnaire = new Questionnaire.Models.Questionnaire
        {
            Id = TypedId<Questionnaire.Models.Questionnaire>.New(),
            Title = "Title",
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Content = new List<Question>
            {
                new TextQuestion { Id = questionId, Title = "Original Question" }
            }
        };

        var delta = new QuestionnaireDelta
        {
            Questions = new Dictionary<TypedId<Question>, QuestionPatch>
            {
                {
                    questionId,
                    new QuestionPatch
                    {
                        Title = Patchable<string>.Set("Updated Question")
                    }
                }
            }
        };

        delta.Apply(questionnaire);

        Assert.Equal("Updated Question", questionnaire.Content[0].Title);
    }

    [Fact]
    public void Delta_CanBeMerged()
    {
        var delta1 = new QuestionnaireDelta
        {
            Title = Patchable<string>.Set("Title 1"),
            IsActive = Patchable<bool>.Set(true)
        };

        var delta2 = new QuestionnaireDelta
        {
            Title = Patchable<string>.Set("Title 2"),
            Description = Patchable<string?>.Set("Description 2")
        };

        var merged = QuestionnaireDelta.Merge(delta1, delta2);

        Assert.True(merged.Title.IsSet);
        Assert.Equal("Title 2", merged.Title.Value);
        Assert.True(merged.IsActive.IsSet);
        Assert.True(merged.IsActive.Value);
        Assert.True(merged.Description.IsSet);
        Assert.Equal("Description 2", merged.Description.Value);
    }
}
