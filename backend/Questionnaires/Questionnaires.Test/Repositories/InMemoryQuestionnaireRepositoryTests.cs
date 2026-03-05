using Microsoft.Extensions.Logging.Abstractions;
using Questionnaires.Contract.Models;
using Questionnaires.Repositories;

namespace Questionnaires.Test.Repositories;

public class InMemoryQuestionnaireRepositoryTests
{
    private readonly IQuestionnaireRepository _repository;

    public InMemoryQuestionnaireRepositoryTests()
    {
        _repository = new InMemoryQuestionnaireRepository(NullLogger<InMemoryQuestionnaireRepository>.Instance);
    }

    [Fact]
    public async Task CreateAsync_CreatesQuestionnaireWithGeneratedId()
    {
        var questionnaire = new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Test Questionnaire",
            Description = "Test Description",
            Content = new List<Question>(),
            CreatedAt = DateTime.UtcNow
        };

        var result = await _repository.CreateAsync(questionnaire);

        Assert.NotNull(result);
        Assert.NotEqual(questionnaire.Id, result.Id); // Repository generates new ID
        Assert.Equal(questionnaire.Title, result.Title);
        Assert.Equal(questionnaire.Description, result.Description);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsQuestionnaire_WhenExists()
    {
        var questionnaire = new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Test",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(questionnaire);
        var retrieved = await _repository.GetByIdAsync(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Title, retrieved.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        var result = await _repository.GetByIdAsync(QuestionnaireId.New());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllQuestionnaires()
    {
        var q1 = await _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Q1",
            CreatedAt = DateTime.UtcNow
        });

        var q2 = await _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Q2",
            CreatedAt = DateTime.UtcNow
        });

        var result = await _repository.GetAllMetaAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, q => q.Id == q1.Id);
        Assert.Contains(result, q => q.Id == q2.Id);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingQuestionnaire()
    {
        var original = await _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Original",
            Description = "Original Description",
            CreatedAt = DateTime.UtcNow
        });

        original.Title = "Updated";
        original.Description = "Updated Description";

        var updated = await _repository.UpdateAsync(original);

        Assert.NotNull(updated);
        Assert.Equal("Updated", updated.Title);
        Assert.Equal("Updated Description", updated.Description);
        Assert.Equal(original.Id, updated.Id);
    }

    [Fact]
    public async Task DeleteAsync_RemovesQuestionnaire()
    {
        var questionnaire = await _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "To Delete",
            CreatedAt = DateTime.UtcNow
        });

        var deleted = await _repository.DeleteAsync(questionnaire.Id);
        var retrieved = await _repository.GetByIdAsync(questionnaire.Id);

        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotExists()
    {
        var result = await _repository.DeleteAsync(QuestionnaireId.New());

        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenExists()
    {
        var questionnaire = await _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = "Exists",
            CreatedAt = DateTime.UtcNow
        });

        var exists = await _repository.ExistsAsync(questionnaire.Id);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenNotExists()
    {
        var exists = await _repository.ExistsAsync(QuestionnaireId.New());

        Assert.False(exists);
    }

    [Fact]
    public async Task Repository_IsConcurrentSafe()
    {
        // Create multiple questionnaires concurrently
        var tasks = Enumerable.Range(0, 100).Select(i => _repository.CreateAsync(new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = $"Concurrent {i}",
            CreatedAt = DateTime.UtcNow
        })).ToArray();

        await Task.WhenAll(tasks);

        var all = await _repository.GetAllMetaAsync();

        Assert.Equal(100, all.Count);
    }
}
