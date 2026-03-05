namespace Questionnaires.Contract.Models;

public class NumberQuestion : Question
{
    public const string QUESTION_TYPE = "Number";
    public override string QuestionType => QUESTION_TYPE;
}

public class NumberQuestionDelta : QuestionDelta
{
    public override string QuestionType => NumberQuestion.QUESTION_TYPE;
}
