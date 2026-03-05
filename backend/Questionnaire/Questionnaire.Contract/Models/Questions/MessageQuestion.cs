namespace Questionnaire.Contract.Models;

public class MessageQuestion : Question
{
    public override string QuestionType => "Message";
}

public class MessageQuestionDelta : QuestionDelta
{
}
