using Microsoft.AspNetCore.Http;

namespace Questionnaire.Contract.Handlers;

public interface IListQuestionnairesHandler
{
    Task<IResult> HandleAsync();
}
