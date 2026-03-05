using Questionnaire.Models;

namespace Questionnaire.Services;

public interface IQuestionnaireService
{
    Task<IEnumerable<Models.Questionnaire>> GetAllQuestionnairesAsync();
    Task<Models.Questionnaire?> GetQuestionnaireByTypeIdAsync(QuestionnaireId typeId);
    Task<Models.Questionnaire> CreateQuestionnaireAsync(string title, string? description, List<Question> content);
    Task<Models.Questionnaire?> UpdateQuestionnaireAsync(QuestionnaireId typeId, string title, string? description);
    Task<bool> DeleteQuestionnaireAsync(QuestionnaireId typeId);
    Task<Models.Questionnaire?> ApplyDeltaAsync(QuestionnaireId typeId, QuestionnaireDelta delta);
}
