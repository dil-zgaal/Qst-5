using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Commands;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Services;
using Core.Model.Delta;

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
        _logger.LogInformation("Handling UpdateQuestionnaireProperty command for questionnaire {Id} at version {Version}", id, questionnaire.Version);

        var delta = new QuestionnaireDelta
        {
            Id = questionnaire.Id,
            FromVersion = questionnaire.Version,
            ToVersion = questionnaire.Version + 1,
            UpdatedAt = DateTime.UtcNow,
            Title = Patchable<string>.Set(command.Title),
            Description = PatchableNullable<string>.Set(command.Description)
        };

        return Task.FromResult(delta);
    }
}
