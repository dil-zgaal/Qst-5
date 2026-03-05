using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Services;

namespace Questionnaires.Handlers;

public class CreateQuestionnaireHandler : ICreateQuestionnaireHandler
{
    private readonly IQuestionnaireUpdateService _updateService;
    private readonly ILogger<CreateQuestionnaireHandler> _logger;

    public CreateQuestionnaireHandler(IQuestionnaireUpdateService updateService, ILogger<CreateQuestionnaireHandler> logger)
    {
        _updateService = updateService;
        _logger = logger;
    }

    public async Task<QuestionnaireDelta> HandleAsync(string title, string? description, List<Question> content)
    {
        _logger.LogInformation("Creating questionnaire: {Title}", title);

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required", nameof(title));
        }

        return await _updateService.CreateAsync(new UpdateQuestionnaireProperty
        {
            Title = title,
            Description = description
        });
    }
}
