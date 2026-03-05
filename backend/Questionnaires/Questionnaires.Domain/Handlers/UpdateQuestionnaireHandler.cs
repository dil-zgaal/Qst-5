using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Services;

namespace Questionnaires.Handlers;

public class UpdateQuestionnaireHandler : IUpdateQuestionnaireHandler
{
    private readonly IQuestionnaireUpdateService _updateService;
    private readonly ILogger<UpdateQuestionnaireHandler> _logger;

    public UpdateQuestionnaireHandler(
        IQuestionnaireUpdateService updateService,
        ILogger<UpdateQuestionnaireHandler> logger)
    {
        _updateService = updateService;
        _logger = logger;
    }

    public async Task<QuestionnaireDelta?> HandleAsync(QuestionnaireId id, UpdateCommand command)
    {
        _logger.LogInformation("Updating questionnaire {Id} with command {CommandType}", id, command.Type);

        var delta = await _updateService.UpdateAsync(id, command);
        return delta;
    }
}
