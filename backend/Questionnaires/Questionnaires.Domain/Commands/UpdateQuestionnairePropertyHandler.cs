using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Commands;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Services;

namespace Questionnaires.Handlers.Commands;

/// <summary>
/// Handler for UpdateQuestionnaireProperty command
/// </summary>
public class UpdateQuestionnairePropertyHandler : ICommandHandler<UpdateQuestionnaireProperty>
{
    private readonly ILogger<UpdateQuestionnairePropertyHandler> _logger;

    public UpdateQuestionnairePropertyHandler(
        ILogger<UpdateQuestionnairePropertyHandler> logger)
    {
        _logger = logger;
    }

    public Task<QuestionnaireDelta> HandleAsync(
        QuestionnaireId id,
        Questionnaire questionnaire,
        UpdateQuestionnaireProperty command)
    {
        _logger.LogInformation("Handling UpdateQuestionnaireProperty command for questionnaire {Id}", id);

        questionnaire.Title = command.Title ?? questionnaire.Title;
        questionnaire.Description = command.Description ?? questionnaire.Description;

        var delta = new QuestionnaireDelta
        {
            Id = questionnaire.Id,
            Title = command.Title != null ? Patchable<string>.Set(questionnaire.Title) : Patchable<string>.NotGiven(),
            Description = command.Description != null ? Patchable<string?>.Set(questionnaire.Description) : Patchable<string?>.NotGiven()
        };

        _logger.LogInformation("Successfully handled UpdateQuestionnaireProperty command for questionnaire {Id}", id);

        return Task.FromResult(delta);
    }
}
