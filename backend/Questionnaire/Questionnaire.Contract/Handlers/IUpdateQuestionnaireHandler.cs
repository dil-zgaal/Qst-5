using Microsoft.AspNetCore.Http;

namespace Questionnaire.Contract.Handlers;

public interface IUpdateQuestionnaireHandler
{
    Task<IResult> HandleAsync(string typeId, string title, string? description);
}
