using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Questionnaires.Repositories;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Commands;
using Questionnaires.Contract.Services;
using Questionnaires.Contract.Repositories;

namespace Questionnaires.Services;

public class QuestionnaireUpdateService : IQuestionnaireUpdateService
{
    private readonly IQuestionnaireRepository _repository;
    private readonly IQuestionnaireCommands _commandsRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QuestionnaireUpdateService> _logger;

    public QuestionnaireUpdateService(
        IQuestionnaireRepository repository,
        IQuestionnaireCommands commandsRepository,
        IServiceProvider serviceProvider,
        ILogger<QuestionnaireUpdateService> logger)
    {
        _repository = repository;
        _commandsRepository = commandsRepository;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<QuestionnaireDelta> CreateAsync(UpdateQuestionnaireProperty command, CancellationToken cancellationToken = default)
    {
        var questionnaire = new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = command.Title,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.CreateAsync(questionnaire, cancellationToken);
        return await _commandsRepository.CreateStreamAsync(questionnaire.Id, command, cancellationToken);
    }

    public async Task<QuestionnaireDelta?> UpdateAsync(
        QuestionnaireId id,
        long fromVersion,
        UpdateCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating questionnaire {Id} with command {CommandType} from version {FromVersion}",
            id, command.Type, fromVersion);

        var newVersion = await _commandsRepository.StoreCommandAsync(id, command, cancellationToken);
        if (newVersion == null)
        {
            _logger.LogWarning("Failed to store command for questionnaire {Id}. The questionnaire may have been deleted.", id);
            return null;
        }

        _logger.LogInformation("Stored command at version {Version} for questionnaire {Id}", newVersion, id);
        await ProcessCommandsAsync(id, newVersion.Value, cancellationToken);

        var aggregatedDelta = await _commandsRepository.GetEffectsAsync(id, fromVersion + 1, newVersion.Value, cancellationToken);

        if (aggregatedDelta == null)
        {
            _logger.LogWarning("No effects found for questionnaire {Id} from version {FromVersion} to {ToVersion}",
                id, fromVersion + 1, newVersion);

            aggregatedDelta = new QuestionnaireDelta
            {
                Id = id,
                FromVersion = fromVersion,
                ToVersion = newVersion.Value,
                UpdatedAt = DateTime.UtcNow
            };
        }

        return aggregatedDelta;
    }

    private async Task ProcessCommandsAsync(QuestionnaireId id, long targetVersion, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Processing commands for questionnaire {Id} up to version {TargetVersion}", id, targetVersion);

        while (true)
        {
            // Lock the next command that needs processing
            var lockedCommand = await _commandsRepository.StartNextCommandAsync(id, TimeSpan.FromMinutes(5), cancellationToken);

            if (lockedCommand == null)
            {
                _logger.LogDebug("No more commands to process for questionnaire {Id}", id);
                break;
            }

            var (version, command, lockId) = lockedCommand.Value;
            _logger.LogInformation("Processing command at version {Version} for questionnaire {Id}", version, id);

            // Get current questionnaire state
            var questionnaire = await _repository.GetByIdAsync(id, cancellationToken);
            if (questionnaire == null)
            {
                _logger.LogError("Questionnaire not found during command processing: {Id}", id);
                break;
            }

            QuestionnaireDelta delta;
            try
            {
                delta = await HandleCommandAsync(id, questionnaire, command, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing command for questionnaire {Id} at version {Version}", id, version);

                // Store empty delta to mark as processed
                delta = new QuestionnaireDelta
                {
                    Id = id,
                    FromVersion = questionnaire.Version,
                    ToVersion = questionnaire.Version + 1,
                    UpdatedAt = DateTime.UtcNow
                };
            }

            // Apply delta to questionnaire and save
            questionnaire.Apply(delta);
            await _commandsRepository.CompleteCommandAsync(id, version, delta, lockId, cancellationToken);
            await _repository.UpdateAsync(questionnaire, cancellationToken);

            // If we've processed the target version, we're done
            if (version >= targetVersion)
            {
                _logger.LogDebug("Reached target version {TargetVersion} for questionnaire {Id}", targetVersion, id);
                break;
            }

        }
    }

    private async Task<QuestionnaireDelta> HandleCommandAsync(
        QuestionnaireId id,
        Questionnaire questionnaire,
        UpdateCommand command,
        CancellationToken cancellationToken)
    {
        return command switch
        {
            UpdateQuestionnaireProperty updateProperty => await HandleCommandAsync<UpdateQuestionnaireProperty>(id, questionnaire, updateProperty),
            _ => throw new ArgumentException($"Unsupported command type: {command.Type}")
        };
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
