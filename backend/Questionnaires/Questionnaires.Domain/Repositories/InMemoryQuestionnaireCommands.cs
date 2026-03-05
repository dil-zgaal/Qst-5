using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Contract.Repositories;
using Core.Model.Delta;

namespace Questionnaires.Repositories;

/// <summary>
/// Concurrent-safe in-memory implementation of questionnaire commands repository
/// Stores commands and effects as JSON strings to emulate database storage
/// </summary>
public class InMemoryQuestionnaireCommands : IQuestionnaireCommands
{
    private class CommandData
    {
        public string CommandJson { get; set; } = "";
        public string? EffectJson { get; set; }
        public Guid? LockId { get; set; }
        public DateTime? LockExpiration { get; set; }
    }

    private class QuestionnaireStream
    {
        private readonly object _lock = new();
        private readonly List<CommandData> _commands = new();

        public long? StoreCommand(string commandJson)
        {
            lock (_lock)
            {
                _commands.Add(new CommandData { CommandJson = commandJson });
                return _commands.Count;
            }
        }

        public (long Version, string CommandJson, Guid LockId)? StartNextCommand(TimeSpan leaseTimeout, DateTime now)
        {
            lock (_lock)
            {
                // Find first command without effect
                for (int i = 0; i < _commands.Count; i++)
                {
                    var cmd = _commands[i];

                    // Already has effect - cannot lock
                    if (cmd.EffectJson != null)
                        continue;

                    // Check if there's a valid lock
                    if (cmd.LockId.HasValue && cmd.LockExpiration.HasValue && cmd.LockExpiration.Value > now)
                        continue; // Lock still valid

                    // Acquire lock
                    var lockId = Guid.NewGuid();
                    cmd.LockId = lockId;
                    cmd.LockExpiration = now.Add(leaseTimeout);

                    return (i + 1L, cmd.CommandJson, lockId);
                }

                return null;
            }
        }

        public bool CompleteCommand(long version, string effectJson, Guid lockId, DateTime now)
        {
            lock (_lock)
            {
                int index = (int)(version - 1);
                if (index < 0 || index >= _commands.Count)
                    return false;

                var cmd = _commands[index];

                // Already has effect
                if (cmd.EffectJson != null)
                    return false;

                // Verify lock
                if (!cmd.LockId.HasValue || cmd.LockId.Value != lockId)
                    return false;

                if (!cmd.LockExpiration.HasValue || cmd.LockExpiration.Value < now)
                    return false; // Lock expired

                // Store effect and release lock
                cmd.EffectJson = effectJson;
                cmd.LockId = null;
                cmd.LockExpiration = null;
                return true;
            }
        }

        public string? GetCommand(long version)
        {
            lock (_lock)
            {
                int index = (int)(version - 1);
                return (index >= 0 && index < _commands.Count) ? _commands[index].CommandJson : null;
            }
        }

        public string? GetEffect(long version)
        {
            lock (_lock)
            {
                int index = (int)(version - 1);
                return (index >= 0 && index < _commands.Count) ? _commands[index].EffectJson : null;
            }
        }

        public long GetLatestVersion()
        {
            lock (_lock)
            {
                return _commands.Count;
            }
        }

        public IReadOnlyList<(long Version, string CommandJson)> GetCommands(long fromVersion, long toVersion)
        {
            lock (_lock)
            {
                var result = new List<(long Version, string CommandJson)>();
                int startIndex = (int)(fromVersion - 1);
                int endIndex = (int)Math.Min(toVersion - 1, _commands.Count - 1);

                for (int i = Math.Max(0, startIndex); i <= endIndex && i < _commands.Count; i++)
                {
                    result.Add((i + 1L, _commands[i].CommandJson));
                }
                return result;
            }
        }

        public IReadOnlyList<(long Version, string EffectJson)> GetEffects(long fromVersion, long toVersion)
        {
            lock (_lock)
            {
                var result = new List<(long Version, string EffectJson)>();
                int startIndex = (int)(fromVersion - 1);
                int endIndex = (int)Math.Min(toVersion - 1, _commands.Count - 1);

                for (int i = Math.Max(0, startIndex); i <= endIndex && i < _commands.Count; i++)
                {
                    var effect = _commands[i].EffectJson;
                    if (effect != null)
                    {
                        result.Add((i + 1L, effect));
                    }
                }
                return result;
            }
        }
    }

    private readonly ConcurrentDictionary<QuestionnaireId, QuestionnaireStream> _streams = new();
    private readonly ILogger<InMemoryQuestionnaireCommands> _logger;

    public InMemoryQuestionnaireCommands(ILogger<InMemoryQuestionnaireCommands> logger)
    {
        _logger = logger;
    }

    public Task<QuestionnaireDelta> CreateStreamAsync(QuestionnaireId id, UpdateQuestionnaireProperty command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating command stream for questionnaire: {Id}", id);

        // Initialize stream
        _streams.TryAdd(id, new QuestionnaireStream());

        // Create initial delta
        var delta = new QuestionnaireDelta
        {
            Id = id,
            FromVersion = 0,
            ToVersion = 0,
            UpdatedAt = DateTime.UtcNow,
            Title = Patchable<string>.Set(command.Title),
            Description = PatchableNullable<string>.Set(command.Description),
        };

        _logger.LogInformation("Created command stream for questionnaire {Id}", id);
        return Task.FromResult(delta);
    }

    public Task<long?> StoreCommandAsync(QuestionnaireId id, UpdateCommand command, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Storing command for questionnaire: {Id}", id);

        if (!_streams.TryGetValue(id, out var stream))
        {
            _logger.LogWarning("Command stream does not exist for questionnaire {Id}", id);
            return Task.FromResult<long?>(null);
        }

        var commandJson = SerializeCommand(command);
        var version = stream.StoreCommand(commandJson);

        _logger.LogInformation("Stored command for questionnaire {Id} at version {Version}", id, version);
        return Task.FromResult(version);
    }

    public Task<IReadOnlyList<(long Version, UpdateCommand Command)>> GetCommandsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting commands for questionnaire {Id} from version {From} to {To}", id, fromVersion, toVersion);

        if (!_streams.TryGetValue(id, out var stream))
        {
            return Task.FromResult<IReadOnlyList<(long Version, UpdateCommand Command)>>(Array.Empty<(long, UpdateCommand)>());
        }

        var commands = stream.GetCommands(fromVersion, toVersion);
        var result = commands
            .Select(c => (c.Version, Command: DeserializeCommand(c.CommandJson)))
            .Where(c => c.Command != null)
            .Select(c => (c.Version, c.Command!))
            .ToList();

        return Task.FromResult<IReadOnlyList<(long Version, UpdateCommand Command)>>(result);
    }

    public Task<(long Version, UpdateCommand Command, Guid LockId)?> StartNextCommandAsync(QuestionnaireId id, TimeSpan leaseTimeout, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Looking for next unlocked command for questionnaire: {Id}", id);

        if (!_streams.TryGetValue(id, out var stream))
        {
            _logger.LogDebug("No command stream found for questionnaire: {Id}", id);
            return Task.FromResult<(long Version, UpdateCommand Command, Guid LockId)?>(null);
        }

        var now = DateTime.UtcNow;
        var locked = stream.StartNextCommand(leaseTimeout, now);

        if (locked == null)
        {
            _logger.LogDebug("No unlocked commands found for questionnaire: {Id}", id);
            return Task.FromResult<(long Version, UpdateCommand Command, Guid LockId)?>(null);
        }

        var (version, commandJson, lockId) = locked.Value;
        var command = DeserializeCommand(commandJson);

        if (command == null)
        {
            _logger.LogError("Failed to deserialize command for questionnaire {Id} at version {Version}", id, version);
            return Task.FromResult<(long Version, UpdateCommand Command, Guid LockId)?>(null);
        }

        _logger.LogInformation("Locked command for questionnaire {Id} at version {Version} with lock {LockId}", id, version, lockId);
        return Task.FromResult<(long Version, UpdateCommand Command, Guid LockId)?>((version, command, lockId));
    }

    public Task<bool> CompleteCommandAsync(QuestionnaireId id, long version, QuestionnaireDelta delta, Guid lockId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Completing command for questionnaire {Id} at version {Version} with lock {LockId}", id, version, lockId);

        if (!_streams.TryGetValue(id, out var stream))
        {
            _logger.LogWarning("Command stream not found for questionnaire {Id}", id);
            return Task.FromResult(false);
        }

        var effectJson = SerializeDelta(delta);
        var now = DateTime.UtcNow;
        var success = stream.CompleteCommand(version, effectJson, lockId, now);

        if (success)
        {
            _logger.LogInformation("Completed command for questionnaire {Id} at version {Version}", id, version);
        }
        else
        {
            _logger.LogWarning("Failed to complete command for questionnaire {Id} at version {Version}", id, version);
        }

        return Task.FromResult(success);
    }

    public Task<QuestionnaireDelta?> GetEffectAsync(QuestionnaireId id, long version, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting effect for questionnaire {Id} at version {Version}", id, version);

        if (!_streams.TryGetValue(id, out var stream))
        {
            return Task.FromResult<QuestionnaireDelta?>(null);
        }

        var effectJson = stream.GetEffect(version);
        if (effectJson == null)
        {
            return Task.FromResult<QuestionnaireDelta?>(null);
        }

        var delta = DeserializeDelta(effectJson);
        return Task.FromResult(delta);
    }

    public Task<QuestionnaireDelta?> GetEffectsAsync(
        QuestionnaireId id,
        long fromVersion,
        long toVersion,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting aggregated effects for questionnaire {Id} from version {From} to {To}", id, fromVersion, toVersion);

        if (!_streams.TryGetValue(id, out var stream))
        {
            return Task.FromResult<QuestionnaireDelta?>(null);
        }

        var effects = stream.GetEffects(fromVersion, toVersion);
        QuestionnaireDelta? aggregated = null;

        foreach (var (version, effectJson) in effects)
        {
            var delta = DeserializeDelta(effectJson);
            if (delta != null)
            {
                if (aggregated == null)
                {
                    aggregated = delta;
                }
                else
                {
                    aggregated.Apply(delta);
                }
            }
        }

        return Task.FromResult(aggregated);
    }

    public Task<long> GetLatestVersionAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting latest version for questionnaire: {Id}", id);

        if (!_streams.TryGetValue(id, out var stream))
        {
            return Task.FromResult(0L);
        }

        var version = stream.GetLatestVersion();
        return Task.FromResult(version);
    }

    public Task<bool> DeleteAsync(QuestionnaireId id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting all commands and effects for questionnaire: {Id}", id);

        var removed = _streams.TryRemove(id, out _);

        if (removed)
        {
            _logger.LogInformation("Deleted command stream for questionnaire {Id}", id);
        }
        else
        {
            _logger.LogDebug("No command stream found for questionnaire {Id}", id);
        }

        return Task.FromResult(removed);
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
}
