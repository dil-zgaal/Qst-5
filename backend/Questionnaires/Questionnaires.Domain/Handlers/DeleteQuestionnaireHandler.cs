using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Services;

namespace Questionnaires.Handlers;

public class DeleteQuestionnaireHandler : IDeleteQuestionnaireHandler
{
    private readonly IQuestionnaireDeleteService _deleteService;
    private readonly ILogger<DeleteQuestionnaireHandler> _logger;

    public DeleteQuestionnaireHandler(IQuestionnaireDeleteService deleteService, ILogger<DeleteQuestionnaireHandler> logger)
    {
        _deleteService = deleteService;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(QuestionnaireId id)
    {
        _logger.LogInformation("Deleting questionnaire {Id}", id);
        return await _deleteService.DeleteAsync(id);
    }
}
