using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Services;

/// <summary>
/// Service for updating questionnaires
/// </summary>
public interface IQuestionnaireUpdateService
{
    /// <summary>
    /// Update a questionnaire using a command
    /// </summary>
    Task<QuestionnaireDelta?> UpdateAsync(QuestionnaireId id, UpdateCommand command, CancellationToken cancellationToken = default);
}
