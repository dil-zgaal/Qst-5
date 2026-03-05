using Questionnaire.Models;

namespace Questionnaire.Repositories;

public class QuestionnaireRepository : IQuestionnaireRepository
{
    private readonly List<Models.Questionnaire> _questionnaires = new();

    public Task<IEnumerable<Models.Questionnaire>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Models.Questionnaire>>(_questionnaires);
    }

    public Task<Models.Questionnaire?> GetByTypeIdAsync(QuestionnaireId typeId)
    {
        var questionnaire = _questionnaires.FirstOrDefault(q => q.Id == typeId);
        return Task.FromResult(questionnaire);
    }

    public Task<Models.Questionnaire> CreateAsync(Models.Questionnaire questionnaire)
    {
        var newQuestionnaire = new Models.Questionnaire
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

    public Task<Models.Questionnaire?> UpdateAsync(QuestionnaireId typeId, Models.Questionnaire questionnaire)
    {
        var existing = _questionnaires.FirstOrDefault(q => q.Id == typeId);
        if (existing == null)
            return Task.FromResult<Models.Questionnaire?>(null);

        existing.Title = questionnaire.Title;
        existing.Description = questionnaire.Description;
        existing.Content = questionnaire.Content;

        return Task.FromResult<Models.Questionnaire?>(existing);
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
