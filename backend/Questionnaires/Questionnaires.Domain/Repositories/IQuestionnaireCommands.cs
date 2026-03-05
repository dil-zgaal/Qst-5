using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Repositories;

/// <summary>
/// Repository for storing and retrieving questionnaire commands and their effects
/// </summary>
public interface IQuestionnaireCommands
{
    /// <summary>
    /// Create a new command stream
    /// </summary>
    Task<QuestionnaireDelta> CreateStreamAsync(QuestionnaireId id, UpdateQuestionnaireProperty command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Store a command and return the new version number, or null if the stream does not exist (e.g. if the questionnaire was deleted)
    /// </summary>
    Task<long?> StoreCommandAsync(QuestionnaireId id, UpdateCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get commands for a questionnaire within a version range
    /// </summary>
    Task<IReadOnlyList<(long Version, UpdateCommand Command)>> GetCommandsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lock the next command without an effect for processing.
    /// Returns the command, version, and a lock ID. Returns null if no commands need processing.
    /// The lock has a timeout after which it will be automatically released.
    /// </summary>
    Task<(long Version, UpdateCommand Command, Guid LockId)?> StartNextCommandAsync(QuestionnaireId id, TimeSpan leaseTimeout, CancellationToken cancellationToken = default);

    /// <summary>
    /// Complete the processing of a command by storing its effect (delta) at a specific version with a lock.
    /// Only succeeds if the provided lockId matches the current lock.
    /// Releases the lock after storing the effect.
    /// </summary>
    Task<bool> CompleteCommandAsync(QuestionnaireId id, long version, QuestionnaireDelta delta, Guid lockId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the effect (delta) for a specific version
    /// </summary>
    Task<QuestionnaireDelta?> GetEffectAsync(QuestionnaireId id, long version, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get aggregated effects (deltas) for a questionnaire within a version range
    /// </summary>
    Task<QuestionnaireDelta?> GetEffectsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the latest version number for a questionnaire
    /// </summary>
    Task<long> GetLatestVersionAsync(QuestionnaireId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete all commands and effects for a questionnaire
    /// </summary>
    Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default);
}
