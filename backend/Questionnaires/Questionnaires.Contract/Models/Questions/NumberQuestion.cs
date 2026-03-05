namespace Questionnaires.Contract.Models;

public class NumberQuestion : Question
{
    public override string QuestionType => "Number";
}

public class NumberQuestionDelta : QuestionDelta
{
}
