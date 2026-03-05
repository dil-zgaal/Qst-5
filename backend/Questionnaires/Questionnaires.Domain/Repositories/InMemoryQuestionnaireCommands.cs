using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Repositories;

namespace Questionnaires.Repositories;

/// <summary>
/// Concurrent-safe in-memory implementation of questionnaire commands repository
/// Stores commands and effects as JSON strings to emulate database storage
/// </summary>
public class InMemoryQuestionnaireCommands : IQuestionnaireCommands
{
    // Store commands: (QuestionnaireId, Version) -> JSON
    private readonly ConcurrentDictionary<(QuestionnaireId, long), string> _commands = new();

    // Store effects: (QuestionnaireId, Version) -> JSON
    private readonly ConcurrentDictionary<(QuestionnaireId, long), string> _effects = new();

    // Track latest version per questionnaire
    private readonly ConcurrentDictionary<QuestionnaireId, long> _versions = new();

    private readonly ILogger<InMemoryQuestionnaireCommands> _logger;

    public InMemoryQuestionnaireCommands(ILogger<InMemoryQuestionnaireCommands> logger)
    {
        _logger = logger;
    }

    private UpdateCommand? DeserializeCommand(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<UpdateCommand>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize command from JSON");
            return null;
        }
    }

    private string SerializeCommand(UpdateCommand command)
    {
        return JsonSerializer.Serialize(command, command.GetType());
    }

    private QuestionnaireDelta? DeserializeDelta(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<QuestionnaireDelta>(json);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize delta from JSON");
            return null;
        }
    }

    private string SerializeDelta(QuestionnaireDelta delta)
    {
        return JsonSerializer.Serialize(delta);
    }

    public Task<long> StoreCommandAsync(QuestionnaireId id, UpdateCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Storing command for questionnaire: {Id}", id);

        // Atomically increment version
        var newVersion = _versions.AddOrUpdate(
            id,
            _ => 1L, // First version
            (_, currentVersion) => currentVersion + 1 // Increment
        );

        // Serialize and store command
        var json = SerializeCommand(command);
        if (!_commands.TryAdd((id, newVersion), json))
        {
            _logger.LogError("Failed to store command for questionnaire {Id} at version {Version}", id, newVersion);
            throw new InvalidOperationException($"Failed to store command for questionnaire {id} at version {newVersion}");
        }

        _logger.LogInformation("Stored command for questionnaire {Id} at version {Version}", id, newVersion);
        return Task.FromResult(newVersion);
    }

    public Task<IReadOnlyList<(long Version, UpdateCommand Command)>> GetCommandsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting commands for questionnaire {Id} from version {From} to {To}", id, fromVersion, toVersion);

        var commands = new List<(long Version, UpdateCommand Command)>();

        for (long version = fromVersion; version <= toVersion; version++)
        {
            if (_commands.TryGetValue((id, version), out var json))
            {
                var command = DeserializeCommand(json);
                if (command != null)
                {
                    commands.Add((version, command));
                }
            }
        }

        return Task.FromResult<IReadOnlyList<(long Version, UpdateCommand Command)>>(commands.AsReadOnly());
    }

    public Task StoreEffectAsync(QuestionnaireId id, long version, QuestionnaireDelta delta, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Storing effect for questionnaire {Id} at version {Version}", id, version);

        var key = (id, version);

        // Check if effect already exists (no-op if it does)
        if (_effects.ContainsKey(key))
        {
            _logger.LogDebug("Effect already exists for questionnaire {Id} at version {Version}, skipping", id, version);
            return Task.CompletedTask;
        }

        // Serialize and store effect
        var json = SerializeDelta(delta);
        _effects.TryAdd(key, json);

        _logger.LogInformation("Stored effect for questionnaire {Id} at version {Version}", id, version);
        return Task.CompletedTask;
    }

    public Task<QuestionnaireDelta?> GetEffectAsync(QuestionnaireId id, long version, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting effect for questionnaire {Id} at version {Version}", id, version);

        if (_effects.TryGetValue((id, version), out var json))
        {
            var delta = DeserializeDelta(json);
            return Task.FromResult(delta);
        }

        return Task.FromResult<QuestionnaireDelta?>(null);
    }

    public Task<QuestionnaireDelta?> GetEffectsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting aggregated effects for questionnaire {Id} from version {From} to {To}", id, fromVersion, toVersion);

        QuestionnaireDelta? aggregated = null;

        for (long version = fromVersion; version <= toVersion; version++)
        {
            if (_effects.TryGetValue((id, version), out var json))
            {
                var delta = DeserializeDelta(json);
                if (delta != null)
                {
                    if (aggregated == null)
                    {
                        aggregated = delta;
                    }
                    else
                    {
                        // Merge deltas (later versions override earlier ones)
                        if (!delta.Title.IsNotGiven)
                            aggregated.Title = delta.Title;
                        if (!delta.Description.IsNotGiven)
                            aggregated.Description = delta.Description;
                        if (delta.Content != null)
                            aggregated.Content = delta.Content;
                        if (!delta.CreatedAt.IsNotGiven)
                            aggregated.CreatedAt = delta.CreatedAt;
                    }
                }
            }
        }

        return Task.FromResult(aggregated);
    }

    public Task<long> GetLatestVersionAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting latest version for questionnaire: {Id}", id);

        if (_versions.TryGetValue(id, out var version))
        {
            return Task.FromResult(version);
        }

        // Return 0 if no version exists yet
        return Task.FromResult(0L);
    }

    public Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting all commands and effects for questionnaire: {Id}", id);

        // Get the latest version to know how many entries to remove
        var hasVersion = _versions.TryGetValue(id, out var latestVersion);

        if (!hasVersion)
        {
            _logger.LogDebug("No commands found for questionnaire {Id}", id);
            return Task.FromResult(false);
        }

        // Remove all commands and effects for this questionnaire
        for (long version = 1; version <= latestVersion; version++)
        {
            _commands.TryRemove((id, version), out _);
            _effects.TryRemove((id, version), out _);
        }

        // Remove version tracking
        _versions.TryRemove(id, out _);

        _logger.LogInformation("Deleted all commands and effects for questionnaire {Id}", id);
        return Task.FromResult(true);
    }
}
