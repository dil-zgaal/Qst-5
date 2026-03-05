using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Services;

/// <summary>
/// Service for updating questionnaires
/// </summary>
public interface IQuestionnaireUpdateService
{
    /// <summary>
    /// Create a new questionnaire
    /// </summary>
    /// <param name="command">The create command, default properties</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created questionnaire delta</returns>
    Task<QuestionnaireDelta> CreateAsync(UpdateQuestionnaireProperty command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a questionnaire using a command
    /// </summary>
    /// <param name="id">The questionnaire ID</param>
    /// <param name="fromVersion">The last known version by the client</param>
    /// <param name="command">The update command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Aggregated delta from client version to latest, and the latest version number</returns>
    Task<QuestionnaireDelta?> UpdateAsync(QuestionnaireId id, long fromVersion, UpdateCommand command, CancellationToken cancellationToken = default);
}
