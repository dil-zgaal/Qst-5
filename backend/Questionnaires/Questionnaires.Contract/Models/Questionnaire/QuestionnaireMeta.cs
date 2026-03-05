namespace Questionnaires.Contract.Models;

/// <summary>
/// Metadata for questionnaire list views
/// </summary>
public class QuestionnaireMeta
{
    public required QuestionnaireId Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required DateTime CreatedAt { get; init; }
}
