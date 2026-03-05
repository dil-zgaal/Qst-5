using System.Text.Json.Serialization;

namespace Questionnaire.Contract.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
[JsonDerivedType(typeof(MessageQuestion), "Message")]
[JsonDerivedType(typeof(TextQuestion), "Text")]
[JsonDerivedType(typeof(NumberQuestion), "Number")]
[JsonDerivedType(typeof(EmailQuestion), "Email")]
public abstract class Question
{
    public required QuestionId Id { get; init; } = QuestionId.New();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> SubQuestions { get; set; } = new();

    [JsonIgnore]
    public abstract string QuestionType { get; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
[JsonDerivedType(typeof(MessageQuestionDelta), "Message")]
[JsonDerivedType(typeof(TextQuestionDelta), "Text")]
[JsonDerivedType(typeof(NumberQuestionDelta), "Number")]
[JsonDerivedType(typeof(EmailQuestionDelta), "Email")]
public class QuestionDelta
{
    public required QuestionId Id { get; init; }
    public Patchable<string> Title { get; set; } = Patchable<string>.NotGiven();
    public Patchable<string?> Description { get; set; } = Patchable<string?>.NotGiven();
    public PatchableArray<QuestionDelta, QuestionId>? SubQuestions { get; set; }

    public void Apply(Question question)
    {
        if (question.Id != Id)
            throw new InvalidOperationException("Delta Id does not match Question Id");

        Title.Apply(question, (q, v) => { if (v != null) q.Title = v; });
        Description.Apply(question, (q, v) => q.Description = v);

        if (SubQuestions != null)
        {
            ApplyQuestionDeltaPatchableArray(question.SubQuestions, SubQuestions);
        }
    }

    private void ApplyQuestionDeltaPatchableArray(List<Question> list, PatchableArray<QuestionDelta, QuestionId> patch)
    {
        switch (patch.Operation)
        {
            case PatchableArrayOperation.Replace:
            case PatchableArrayOperation.Add:
            case PatchableArrayOperation.AddRange:
                throw new NotSupportedException("Add/Replace operations not supported for QuestionDelta patches. Use full Question objects.");

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
