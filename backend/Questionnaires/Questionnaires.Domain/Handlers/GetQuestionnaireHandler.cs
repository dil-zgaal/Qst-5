using Microsoft.Extensions.Logging;
using Questionnaires.Repositories;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;

namespace Questionnaires.Handlers;

public class GetQuestionnaireHandler : IGetQuestionnaireHandler
{
    private readonly IQuestionnaireRepository _repository;
    private readonly ILogger<GetQuestionnaireHandler> _logger;

    public GetQuestionnaireHandler(IQuestionnaireRepository repository, ILogger<GetQuestionnaireHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Questionnaire?> HandleAsync(QuestionnaireId id)
    {
        _logger.LogDebug("Getting questionnaire {Id}", id);
        return await _repository.GetByIdAsync(id);
    }
}
