using System.Text.Json.Serialization;

namespace Questionnaire.Contract.Models;

public class Questionnaire
{
    public required QuestionnaireId Id { get; init; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> Content { get; set; } = new();
    public required DateTime CreatedAt { get; init; }
}

public class QuestionnaireDelta
{
    public required QuestionnaireId Id { get; init; }
    public Patchable<string> Title { get; set; } = Patchable<string>.NotGiven();
    public Patchable<string?> Description { get; set; } = Patchable<string?>.NotGiven();
    public PatchableArray<QuestionDelta, QuestionId>? Content { get; set; }
    public Patchable<DateTime> CreatedAt { get; set; } = Patchable<DateTime>.NotGiven();

    public void Apply(Questionnaire questionnaire)
    {
        if (questionnaire.Id != Id)
            throw new InvalidOperationException("Delta Id does not match Questionnaire Id");

        // Apply questionnaire-level property patches
        Title.Apply(questionnaire, (q, v) => { if (v != null) q.Title = v; });
        Description.Apply(questionnaire, (q, v) => q.Description = v);
        CreatedAt.Apply(questionnaire, (q, v) => { /* CreatedAt is init-only, cannot be changed */ });

        // Apply content array patch
        if (Content != null)
        {
            ApplyQuestionDeltaPatchableArray(questionnaire.Content, Content);
        }
    }

    public static QuestionnaireDelta Merge(QuestionnaireId typeId, params QuestionnaireDelta[] deltas)
    {
        if (deltas.Any(d => d.Id != typeId))
            throw new InvalidOperationException("All deltas must have the same Id");

        var merged = new QuestionnaireDelta { Id = typeId };

        // Merge property patches (later patches override earlier ones)
        foreach (var delta in deltas)
        {
            if (!delta.Title.IsNotGiven)
                merged.Title = delta.Title;

            if (!delta.Description.IsNotGiven)
                merged.Description = delta.Description;

            if (!delta.CreatedAt.IsNotGiven)
                merged.CreatedAt = delta.CreatedAt;

            // For Content, only keep the last one (arrays are replaced, not merged individually)
            if (delta.Content != null)
                merged.Content = delta.Content;
        }

        return merged;
    }

    private void ApplyQuestionDeltaPatchableArray(List<Question> list, PatchableArray<QuestionDelta, QuestionId> patch)
    {
        switch (patch.Operation)
        {
            case PatchableArrayOperation.Replace:
                // Not supported for deltas - would need full questions
                throw new NotSupportedException("Replace operation not supported for QuestionDelta patches");

            case PatchableArrayOperation.Add:
            case PatchableArrayOperation.AddRange:
                // Not supported for deltas in this simplified model - adding questions requires full Question objects
                throw new NotSupportedException("Add operations not supported for QuestionDelta patches. Use full Question objects.");

            case PatchableArrayOperation.Remove:
                if (patch.Index.HasValue && patch.Index.Value >= 0 && patch.Index.Value < list.Count)
                    list.RemoveAt(patch.Index.Value);
                break;

            case PatchableArrayOperation.RemoveById:
                if (patch.ItemId.HasValue)
                {
                    var question = list.FirstOrDefault(q => q.Id == patch.ItemId.Value);
                    if (question != null)
                        list.Remove(question);
                }
                break;

            case PatchableArrayOperation.RemoveRange:
                if (patch.Index.HasValue && patch.Count.HasValue &&
                    patch.Index.Value >= 0 && patch.Index.Value < list.Count)
                {
                    var count = Math.Min(patch.Count.Value, list.Count - patch.Index.Value);
                    list.RemoveRange(patch.Index.Value, count);
                }
                break;

            case PatchableArrayOperation.Move:
                if (patch.Index.HasValue && patch.ToIndex.HasValue &&
                    patch.Index.Value >= 0 && patch.Index.Value < list.Count &&
                    patch.ToIndex.Value >= 0 && patch.ToIndex.Value < list.Count)
                {
                    var item = list[patch.Index.Value];
                    list.RemoveAt(patch.Index.Value);
                    list.Insert(patch.ToIndex.Value, item);
                }
                break;

            case PatchableArrayOperation.MoveById:
                if (patch.ItemId.HasValue && patch.ToIndex.HasValue)
                {
                    var question = list.FirstOrDefault(q => q.Id == patch.ItemId.Value);
                    if (question != null)
                    {
                        list.Remove(question);
                        var toIndex = Math.Min(patch.ToIndex.Value, list.Count);
                        list.Insert(toIndex, question);
                    }
                }
                break;

            case PatchableArrayOperation.Clear:
                list.Clear();
                break;
        }

        // After structural operations, apply any deltas to existing questions
        if (patch.Item != null)
        {
            var existing = list.FirstOrDefault(q => q.Id == patch.Item.Id);
            if (existing != null)
                patch.Item.Apply(existing);
        }

        if (patch.Items != null)
        {
            foreach (var delta in patch.Items)
            {
                var existing = list.FirstOrDefault(q => q.Id == delta.Id);
                if (existing != null)
                    delta.Apply(existing);
            }
        }
    }
}
