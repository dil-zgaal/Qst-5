namespace Questionnaires.Contract.Handlers;

public interface IDeleteQuestionnaireHandler
{
    Task<bool> HandleAsync(QuestionnaireId id);
}
