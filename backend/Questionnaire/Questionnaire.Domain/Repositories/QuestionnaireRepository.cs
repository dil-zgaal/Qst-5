using Questionnaire.Contract.Models;

namespace Questionnaire.Repositories;

public class QuestionnaireRepository : IQuestionnaireRepository
{
    private readonly List<Contract.Models.Questionnaire> _questionnaires = new();

    public Task<IEnumerable<Contract.Models.Questionnaire>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Contract.Models.Questionnaire>>(_questionnaires);
    }

    public Task<Contract.Models.Questionnaire?> GetByTypeIdAsync(QuestionnaireId typeId)
    {
        var questionnaire = _questionnaires.FirstOrDefault(q => q.Id == typeId);
        return Task.FromResult(questionnaire);
    }

    public Task<Contract.Models.Questionnaire> CreateAsync(Contract.Models.Questionnaire questionnaire)
    {
        var newQuestionnaire = new Contract.Models.Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Content = questionnaire.Content,
            CreatedAt = DateTime.UtcNow
        };

        _questionnaires.Add(newQuestionnaire);
        return Task.FromResult(newQuestionnaire);
    }

    public Task<Contract.Models.Questionnaire?> UpdateAsync(QuestionnaireId typeId, Contract.Models.Questionnaire questionnaire)
    {
        var existing = _questionnaires.FirstOrDefault(q => q.Id == typeId);
        if (existing == null)
            return Task.FromResult<Contract.Models.Questionnaire?>(null);

        existing.Title = questionnaire.Title;
        existing.Description = questionnaire.Description;
        existing.Content = questionnaire.Content;

        return Task.FromResult<Contract.Models.Questionnaire?>(existing);
    }

    public Task<bool> DeleteAsync(QuestionnaireId typeId)
    {
        var questionnaire = _questionnaires.FirstOrDefault(q => q.Id == typeId);
        if (questionnaire == null)
            return Task.FromResult(false);

        _questionnaires.Remove(questionnaire);
        return Task.FromResult(true);
    }
}
