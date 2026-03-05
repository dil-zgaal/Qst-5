namespace Questionnaires.Contract.Models;

public class EmailQuestion : Question
{
    public override string QuestionType => "Email";
}

public class EmailQuestionDelta : QuestionDelta
{
}
