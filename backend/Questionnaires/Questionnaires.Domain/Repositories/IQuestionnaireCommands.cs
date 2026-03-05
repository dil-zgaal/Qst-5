using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Repositories;

/// <summary>
/// Repository for storing and retrieving questionnaire commands and their effects
/// </summary>
public interface IQuestionnaireCommands
{
    /// <summary>
    /// Store a command and return the new version number
    /// </summary>
    Task<long> StoreCommandAsync(QuestionnaireId id, UpdateCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get commands for a questionnaire within a version range
    /// </summary>
    Task<IReadOnlyList<(long Version, UpdateCommand Command)>> GetCommandsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Store the effect (delta) of a command at a specific version.
    /// This is a no-op if an effect has already been stored for this version.
    /// </summary>
    Task StoreEffectAsync(QuestionnaireId id, long version, QuestionnaireDelta delta, CancellationToken cancellationToken = default);

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
