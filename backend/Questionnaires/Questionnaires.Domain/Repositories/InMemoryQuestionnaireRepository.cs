using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Models;

namespace Questionnaires.Repositories;

/// <summary>
/// Concurrent-safe in-memory implementation of questionnaire repository
/// Stores questionnaires as JSON strings to emulate database storage operations
/// </summary>
public class InMemoryQuestionnaireRepository : IQuestionnaireRepository
{
    private readonly ConcurrentDictionary<QuestionnaireId, string> _store = new();
    private readonly ILogger<InMemoryQuestionnaireRepository> _logger;

    public InMemoryQuestionnaireRepository(ILogger<InMemoryQuestionnaireRepository> logger)
    {
        _logger = logger;
    }

    public Task<IReadOnlyList<QuestionnaireMeta>> GetAllMetaAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all questionnaire metadata. Count: {Count}", _store.Count);

        // Deserialize all questionnaires from JSON and project to metadata
        var questionnaires = _store.Values
            .Select(json => Deserialize(json))
            .Where(q => q != null)
            .Cast<Questionnaire>()
            .OrderBy(q => q.CreatedAt)
            .Select(q => new QuestionnaireMeta
            {
                Id = q.Id,
                Version = q.Version,
                Title = q.Title,
                Description = q.Description,
                CreatedAt = q.CreatedAt,
                UpdatedAt = q.UpdatedAt,
            })
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyList<QuestionnaireMeta>>(questionnaires);
    }

    public Task<Questionnaire?> GetByIdAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting questionnaire by ID: {Id}", id);

        // Deserialize from JSON
        if (_store.TryGetValue(id, out var json))
        {
            var questionnaire = Deserialize(json);
            return Task.FromResult(questionnaire);
        }

        return Task.FromResult<Questionnaire?>(null);
    }

    public Task<Questionnaire> CreateAsync(Questionnaire questionnaire, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating questionnaire: {Title}", questionnaire.Title);

        // Create a new questionnaire with generated ID and timestamp
        var newQuestionnaire = new Questionnaire
        {
            Id = QuestionnaireId.New(),
            Title = questionnaire.Title,
            Description = questionnaire.Description,
            Content = new List<Question>(questionnaire.Content), // Deep copy the list
            CreatedAt = DateTime.UtcNow,
            Version = 0 // New questionnaires start at version 0
        };

        // Serialize to JSON before storing
        var json = Serialize(newQuestionnaire);

        if (!_store.TryAdd(newQuestionnaire.Id, json))
        {
            _logger.LogError("Failed to add questionnaire with ID: {Id}", newQuestionnaire.Id);
            throw new InvalidOperationException($"Failed to create questionnaire with ID: {newQuestionnaire.Id}");
        }

        _logger.LogInformation("Created questionnaire with ID: {Id}", newQuestionnaire.Id);
        return Task.FromResult(newQuestionnaire);
    }

    public Task<Questionnaire?> UpdateAsync(Questionnaire questionnaire, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating questionnaire with ID: {Id}", questionnaire.Id);

        var updatedJson = _store.AddOrUpdate(
            questionnaire.Id,
            // Add factory (shouldn't be called since we're updating)
            _ => Serialize(questionnaire),
            // Update factory - deserialize, update, serialize
            (_, existingJson) =>
            {
                var existing = Deserialize(existingJson);
                if (existing != null)
                {
                    // Update mutable properties
                    existing.Title = questionnaire.Title;
                    existing.Description = questionnaire.Description;
                    existing.Content = new List<Question>(questionnaire.Content); // Deep copy
                    existing.Version = questionnaire.Version;
                }
                return Serialize(existing ?? questionnaire);
            });

        _logger.LogInformation("Updated questionnaire with ID: {Id}", questionnaire.Id);

        // Deserialize the result to return
        var result = Deserialize(updatedJson);
        return Task.FromResult(result);
    }

    public Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting questionnaire with ID: {Id}", id);

        var removed = _store.TryRemove(id, out _);

        if (removed)
        {
            _logger.LogInformation("Deleted questionnaire with ID: {Id}", id);
        }
        else
        {
            _logger.LogWarning("Questionnaire not found for deletion: {Id}", id);
        }

        return Task.FromResult(removed);
    }

    private Questionnaire? Deserialize(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Questionnaire>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize questionnaire from JSON");
            return null;
        }
    }

    private string Serialize(Questionnaire questionnaire)
    {
        return JsonSerializer.Serialize(questionnaire);
    }

}
