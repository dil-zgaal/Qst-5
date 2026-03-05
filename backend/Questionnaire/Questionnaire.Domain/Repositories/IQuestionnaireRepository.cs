using Questionnaire.Contract.Models;

namespace Questionnaire.Repositories;

public interface IQuestionnaireRepository
{
    Task<IEnumerable<Contract.Models.Questionnaire>> GetAllAsync();
    Task<Contract.Models.Questionnaire?> GetByTypeIdAsync(QuestionnaireId typeId);
    Task<Contract.Models.Questionnaire> CreateAsync(Contract.Models.Questionnaire questionnaire);
    Task<Contract.Models.Questionnaire?> UpdateAsync(QuestionnaireId typeId, Contract.Models.Questionnaire questionnaire);
    Task<bool> DeleteAsync(QuestionnaireId typeId);
}
