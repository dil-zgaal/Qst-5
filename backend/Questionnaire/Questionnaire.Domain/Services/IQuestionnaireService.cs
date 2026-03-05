using Questionnaire.Contract.Models;

namespace Questionnaire.Services;

public interface IQuestionnaireService
{
    Task<IEnumerable<Contract.Models.Questionnaire>> GetAllQuestionnairesAsync();
    Task<Contract.Models.Questionnaire?> GetQuestionnaireByTypeIdAsync(QuestionnaireId typeId);
    Task<Contract.Models.Questionnaire> CreateQuestionnaireAsync(string title, string? description, List<Question> content);
    Task<Contract.Models.Questionnaire?> UpdateQuestionnaireAsync(QuestionnaireId typeId, string title, string? description);
    Task<bool> DeleteQuestionnaireAsync(QuestionnaireId typeId);
    Task<Contract.Models.Questionnaire?> ApplyDeltaAsync(QuestionnaireId typeId, QuestionnaireDelta delta);
}
