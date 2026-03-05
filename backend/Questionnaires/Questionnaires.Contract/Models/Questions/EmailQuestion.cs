namespace Questionnaires.Contract.Models;

public class EmailQuestion : Question
{
    public const string QUESTION_TYPE = "Email";
    public override string QuestionType => QUESTION_TYPE;
}

public class EmailQuestionDelta : QuestionDelta
{
    public override string QuestionType => EmailQuestion.QUESTION_TYPE;
}
