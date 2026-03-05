namespace Questionnaires.Contract.Services;

/// <summary>
/// Service for deleting questionnaires
/// </summary>
public interface IQuestionnaireDeleteService
{
    /// <summary>
    /// Delete a questionnaire by ID
    /// </summary>
    Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default);
}
