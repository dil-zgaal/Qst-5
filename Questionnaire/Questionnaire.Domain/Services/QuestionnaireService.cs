using Questionnaire.Models;
using Questionnaire.Repositories;

namespace Questionnaire.Services;

public class QuestionnaireService : IQuestionnaireService
{
    private readonly IQuestionnaireRepository _repository;
    private readonly ILogger<QuestionnaireService> _logger;

    public QuestionnaireService(IQuestionnaireRepository repository, ILogger<QuestionnaireService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Models.Questionnaire>> GetAllQuestionnairesAsync()
    {
        _logger.LogDebug("Getting all questionnaires");
        return await _repository.GetAllAsync();
    }

    public async Task<Models.Questionnaire?> GetQuestionnaireByTypeIdAsync(QuestionnaireId typeId)
    {
        _logger.LogDebug("Getting questionnaire with ID: {Id}", typeId);
        return await _repository.GetByTypeIdAsync(typeId);
    }

    public async Task<Models.Questionnaire> CreateQuestionnaireAsync(string title, string? description, List<Question> content)
    {
        _logger.LogInformation("Creating questionnaire: {Title}", title);

        var questionnaire = new Models.Questionnaire
        {
            Id = QuestionnaireId.New(), // Temporary, will be set by repository
            Title = title,
            Description = description,
            Content = content,
            CreatedAt = DateTime.UtcNow // Temporary, will be set by repository
        };

        return await _repository.CreateAsync(questionnaire);
    }

    public async Task<Models.Questionnaire?> UpdateQuestionnaireAsync(QuestionnaireId typeId, string title, string? description)
    {
        _logger.LogInformation("Updating questionnaire with ID: {Id}", typeId);

        var questionnaire = new Models.Questionnaire
        {
            Id = typeId,
            Title = title,
            Description = description,
            CreatedAt = DateTime.UtcNow // Will not be used in update
        };

        return await _repository.UpdateAsync(typeId, questionnaire);
    }

    public async Task<bool> DeleteQuestionnaireAsync(QuestionnaireId typeId)
    {
        _logger.LogInformation("Deleting questionnaire with ID: {Id}", typeId);
        return await _repository.DeleteAsync(typeId);
    }

    public async Task<Models.Questionnaire?> ApplyDeltaAsync(QuestionnaireId typeId, QuestionnaireDelta delta)
    {
        _logger.LogInformation("Applying delta to questionnaire with ID: {Id}", typeId);

        var questionnaire = await _repository.GetByTypeIdAsync(typeId);
        if (questionnaire == null)
            return null;

        delta.Apply(questionnaire);

        return await _repository.UpdateAsync(typeId, questionnaire);
    }
}
