using Microsoft.AspNetCore.Http;

namespace Questionnaire.Contract.Handlers;

public interface IGetQuestionnaireHandler
{
    Task<IResult> HandleAsync(string typeId);
}
