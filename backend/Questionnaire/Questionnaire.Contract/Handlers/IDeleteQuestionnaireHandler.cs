using Microsoft.AspNetCore.Http;

namespace Questionnaire.Contract.Handlers;

public interface IDeleteQuestionnaireHandler
{
    Task<IResult> HandleAsync(string typeId);
}
