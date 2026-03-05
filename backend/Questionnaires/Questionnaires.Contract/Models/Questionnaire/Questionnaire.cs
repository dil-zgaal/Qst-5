using System.Text.Json.Serialization;
using Core.Model.Delta;

namespace Questionnaires.Contract.Models;

public class Questionnaire
{
    public required QuestionnaireId Id { get; init; }
    public long Version { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> Content { get; set; } = new();

    public void Apply(QuestionnaireDelta delta)
    {
        if (Id != delta.Id)
            throw new InvalidOperationException("Delta Id does not match Questionnaire Id");

        if (Version != delta.FromVersion)
        {
            throw new InvalidOperationException($"Questionnaire version {Version} does not match delta from version {delta.FromVersion}");
        }

        // Apply version
        Version = delta.ToVersion;
        UpdatedAt = delta.UpdatedAt;

        // Apply questionnaire-level property patches
        delta.Title.Patch((v) => { Title = v; });
        delta.Description.Patch((v) => { Description = v; });
    }
}

public class QuestionnaireDelta
{
    public required QuestionnaireId Id { get; init; }
    public long FromVersion { get; set; }
    public long ToVersion { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Patchable<string> Title { get; set; } = Patchable<string>.NotGiven();
    public PatchableNullable<string> Description { get; set; } = PatchableNullable<string>.NotGiven();

    public void Apply(QuestionnaireDelta delta)
    {
        if (delta.Id != Id)
            throw new InvalidOperationException("Delta Id does not match Questionnaire Id");

        if (ToVersion != delta.FromVersion)
            throw new InvalidOperationException($"Delta from version {delta.FromVersion} does not match to version {ToVersion}");

        ToVersion = delta.ToVersion;
        UpdatedAt = delta.UpdatedAt;

        Title.Apply(delta.Title);
        Description.Apply(delta.Description);
    }
}
