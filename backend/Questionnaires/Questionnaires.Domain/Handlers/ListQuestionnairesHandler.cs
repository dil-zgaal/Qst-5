using Microsoft.Extensions.Logging;
using Questionnaires.Repositories;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;

namespace Questionnaires.Handlers;

public class ListQuestionnairesHandler : IListQuestionnairesHandler
{
    private readonly IQuestionnaireRepository _repository;
    private readonly ILogger<ListQuestionnairesHandler> _logger;

    public ListQuestionnairesHandler(IQuestionnaireRepository repository, ILogger<ListQuestionnairesHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<QuestionnaireMeta>> HandleAsync()
    {
        _logger.LogDebug("Listing all questionnaires");
        return await _repository.GetAllMetaAsync();
    }
}
