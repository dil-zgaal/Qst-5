using Microsoft.Extensions.Logging;
using Questionnaires.Repositories;
using Questionnaires.Contract.Services;
using Questionnaires.Contract.Repositories;

namespace Questionnaires.Services;

public class QuestionnaireDeleteService : IQuestionnaireDeleteService
{
    private readonly IQuestionnaireRepository _repository;
    private readonly IQuestionnaireCommands _commandsRepository;
    private readonly ILogger<QuestionnaireDeleteService> _logger;

    public QuestionnaireDeleteService(
        IQuestionnaireRepository repository,
        IQuestionnaireCommands commandsRepository,
        ILogger<QuestionnaireDeleteService> logger)
    {
        _repository = repository;
        _commandsRepository = commandsRepository;
        _logger = logger;
    }

    public async Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting questionnaire {Id}", id);

        // Delete commands and effects first
        await _commandsRepository.DeleteAsync(id, cancellationToken);

        // Delete the questionnaire itself
        var deleted = await _repository.DeleteAsync(id, cancellationToken);

        if (deleted)
        {
            _logger.LogInformation("Successfully deleted questionnaire {Id} and its commands", id);
        }
        else
        {
            _logger.LogWarning("Questionnaire {Id} not found for deletion", id);
        }

        return deleted;
    }
}
