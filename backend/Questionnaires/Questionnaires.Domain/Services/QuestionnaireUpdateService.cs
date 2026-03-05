using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Questionnaires.Repositories;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Commands;
using Questionnaires.Contract.Services;

namespace Questionnaires.Services;

public class QuestionnaireUpdateService : IQuestionnaireUpdateService
{
    private readonly IQuestionnaireRepository _repository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuestionnaireUpdateService> _logger;

    public QuestionnaireUpdateService(
        IQuestionnaireRepository repository,
        IServiceProvider serviceProvider,
        ILogger<QuestionnaireUpdateService> logger)
    {
        _repository = repository;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<QuestionnaireDelta?> UpdateAsync(
        QuestionnaireId id,
        UpdateCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating questionnaire {Id} with command {CommandType}", id, command.Type);

        // Get existing questionnaire
        var questionnaire = await _repository.GetByIdAsync(id, cancellationToken);
        if (questionnaire == null)
        {
            _logger.LogWarning("Questionnaire not found: {Id}", id);
            return null;
        }

        // Dispatch to specific command handler based on command type
        var delta = command switch
        {
            UpdateQuestionnaireProperty updateProperty => await HandleCommandAsync(id, questionnaire, updateProperty),
            _ => throw new ArgumentException($"Unsupported command type: {command.Type}")
        };

        // Save changes
        await _repository.UpdateAsync(questionnaire, cancellationToken);

        return delta;
    }

    private async Task<QuestionnaireDelta> HandleCommandAsync<TCommand>(
        QuestionnaireId id,
        Questionnaire questionnaire,
        TCommand command)
        where TCommand : UpdateCommand
    {
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();

        if (handler == null)
        {
            throw new InvalidOperationException($"No handler registered for command type: {typeof(TCommand).Name}");
        }

        return await handler.HandleAsync(id, questionnaire, command);
    }
}
