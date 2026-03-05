using Microsoft.Extensions.Logging;
using Questionnaires.Repositories;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;

namespace Questionnaires.Handlers;

public class CreateQuestionnaireHandler : ICreateQuestionnaireHandler
{
    private readonly IQuestionnaireRepository _repository;
    private readonly ILogger<CreateQuestionnaireHandler> _logger;

    public CreateQuestionnaireHandler(IQuestionnaireRepository repository, ILogger<CreateQuestionnaireHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<QuestionnaireDelta> HandleAsync(string title, string? description, List<Question> content)
    {
        _logger.LogInformation("Creating questionnaire: {Title}", title);

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required", nameof(title));
        }

        var questionnaire = new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = title,
            Description = description,
            Content = content ?? new List<Question>(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.CreateAsync(questionnaire);

        var delta = new QuestionnaireDelta
        {
            Id = created.Id,
            Title = Patchable<string>.Set(created.Title),
            Description = Patchable<string?>.Set(created.Description),
            CreatedAt = Patchable<DateTime>.Set(created.CreatedAt)
        };

        return delta;
    }
}
