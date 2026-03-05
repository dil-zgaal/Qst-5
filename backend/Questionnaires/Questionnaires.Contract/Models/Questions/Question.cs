using System.Text.Json.Serialization;
using Core.Model.Delta;

namespace Questionnaires.Contract.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
[JsonDerivedType(typeof(MessageQuestion), MessageQuestion.QUESTION_TYPE)]
[JsonDerivedType(typeof(TextQuestion), TextQuestion.QUESTION_TYPE)]
[JsonDerivedType(typeof(NumberQuestion), NumberQuestion.QUESTION_TYPE)]
[JsonDerivedType(typeof(EmailQuestion), EmailQuestion.QUESTION_TYPE)]
public abstract class Question
{
    [JsonIgnore]
    public abstract string QuestionType { get; }

    public required QuestionId Id { get; init; } = QuestionId.New();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> SubQuestions { get; set; } = new();


    public virtual void Apply(QuestionDelta delta)
    {
        if (delta.Id != Id)
            throw new InvalidOperationException("Delta Id does not match Question Id");

        delta.Title.Patch(v => Title = v);
        delta.Description.Patch(v => Description = v);
    }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
[JsonDerivedType(typeof(MessageQuestionDelta), "Message")]
[JsonDerivedType(typeof(TextQuestionDelta), "Text")]
[JsonDerivedType(typeof(NumberQuestionDelta), "Number")]
[JsonDerivedType(typeof(EmailQuestionDelta), "Email")]
public abstract class QuestionDelta
{
    [JsonIgnore]
    public abstract string QuestionType { get; }

    public required QuestionId Id { get; init; }
    public Patchable<string> Title { get; set; } = Patchable<string>.NotGiven();
    public PatchableNullable<string> Description { get; set; } = PatchableNullable<string>.NotGiven();

    public virtual void Apply(QuestionDelta delta)
    {
        if (delta.QuestionType != QuestionType)
            throw new InvalidOperationException("Delta QuestionType does not match Question Type");
        if (delta.Id != Id)
            throw new InvalidOperationException("Delta Id does not match Question Id");

        Title.Apply(delta.Title);
        Description.Apply(delta.Description);
    }
}
