namespace Questionnaires.Contract.Models;

public class TextQuestion : Question
{
    public override string QuestionType => "Text";
}

public class TextQuestionDelta : QuestionDelta
{
}
