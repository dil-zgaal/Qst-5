using Questionnaire.Models;

namespace Questionnaire.Repositories;

public interface IQuestionnaireRepository
{
    Task<IEnumerable<Models.Questionnaire>> GetAllAsync();
    Task<Models.Questionnaire?> GetByTypeIdAsync(QuestionnaireId typeId);
    Task<Models.Questionnaire> CreateAsync(Models.Questionnaire questionnaire);
    Task<Models.Questionnaire?> UpdateAsync(QuestionnaireId typeId, Models.Questionnaire questionnaire);
    Task<bool> DeleteAsync(QuestionnaireId typeId);
}
