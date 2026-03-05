using Questionnaires.Contract.Models;

namespace Questionnaires.Contract.Handlers;

public interface IGetQuestionnaireHandler
{
    Task<Questionnaire?> HandleAsync(QuestionnaireId id);
}
