using Microsoft.Extensions.Logging;
using Questionnaires.Repositories;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Repositories;

namespace Questionnaires.Handlers;

public class GetQuestionnaireHandler : IGetQuestionnaireHandler
{
    private readonly IQuestionnaireRepository _repository;
    private readonly IQuestionnaireCommands _commandsRepository;
    private readonly ILogger<GetQuestionnaireHandler> _logger;

    public GetQuestionnaireHandler(
        IQuestionnaireRepository repository,
        IQuestionnaireCommands commandsRepository,
        ILogger<GetQuestionnaireHandler> logger)
    {
        _repository = repository;
        _commandsRepository = commandsRepository;
        _logger = logger;
    }

    public async Task<Questionnaire?> HandleAsync(QuestionnaireId id)
    {
        _logger.LogDebug("Getting questionnaire {Id}", id);

        var questionnaire = await _repository.GetByIdAsync(id);
        if (questionnaire == null)
        {
            return null;
        }

        // Version is already on the questionnaire model
        return questionnaire;
    }
}
