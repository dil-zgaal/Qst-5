using Microsoft.AspNetCore.Http;

namespace Questionnaire.Contract.Handlers;

public interface ICreateQuestionnaireHandler
{
    Task<IResult> HandleAsync(string title, string? description, List<Models.Question> content);
}
