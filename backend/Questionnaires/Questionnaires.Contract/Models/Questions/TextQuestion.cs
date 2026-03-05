namespace Questionnaires.Contract.Models;

public class TextQuestion : Question
{
    public const string QUESTION_TYPE = "Text";
    public override string QuestionType => QUESTION_TYPE;
}

public class TextQuestionDelta : QuestionDelta
{
    public override string QuestionType => TextQuestion.QUESTION_TYPE;
}
