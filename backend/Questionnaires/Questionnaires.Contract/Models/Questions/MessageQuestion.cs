namespace Questionnaires.Contract.Models;

public class MessageQuestion : Question
{
    public const string QUESTION_TYPE = "Message";
    public override string QuestionType => QUESTION_TYPE;
}

public class MessageQuestionDelta : QuestionDelta
{
    public override string QuestionType => MessageQuestion.QUESTION_TYPE;
}
