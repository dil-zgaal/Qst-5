using Questionnaires.Contract.Models;

namespace Questionnaires.Repositories;

/// <summary>
/// Repository interface for managing Questionnaire persistence
/// </summary>
public interface IQuestionnaireRepository
{
    /// <summary>
    /// Get all questionnaire metadata
    /// </summary>
    Task<IReadOnlyList<QuestionnaireMeta>> GetAllMetaAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a questionnaire by its ID
    /// </summary>
    Task<Questionnaire?> GetByIdAsync(QuestionnaireId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new questionnaire
    /// </summary>
    Task<Questionnaire> CreateAsync(Questionnaire questionnaire, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing questionnaire
    /// </summary>
    Task<Questionnaire?> UpdateAsync(Questionnaire questionnaire, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a questionnaire by ID
    /// </summary>
    Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a questionnaire exists
    /// </summary>
    Task<bool> ExistsAsync(QuestionnaireId id, CancellationToken cancellationToken = default);
}
