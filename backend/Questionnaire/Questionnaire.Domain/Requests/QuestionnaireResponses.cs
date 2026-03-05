using Questionnaire.Contract.Models;

namespace Questionnaire.Requests;

public class QuestionnaireResponse
{
    public required Contract.Models.Questionnaire Questionnaire { get; set; }
}

public class QuestionnaireUpdatedResponse
{
    public required QuestionnaireDelta Delta { get; set; }
}

public class QuestionnaireListItemResponse
{
    public required QuestionnaireId Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required DateTime CreatedAt { get; set; }
}

public class QuestionnaireListResponse
{
    public required List<QuestionnaireListItemResponse> Questionnaires { get; set; }
}
