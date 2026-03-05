using Questionnaire.Contract.Handlers;
using Questionnaire.Contract.Models;

namespace Questionnaire.Requests;

public class CreateQuestionnaireRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> Content { get; set; } = new();
}

public static class CreateQuestionnaire
{
    public static RouteHandlerBuilder MapCreateQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async (CreateQuestionnaireRequest request, ICreateQuestionnaireHandler handler) =>
            await handler.HandleAsync(request.Title, request.Description, request.Content))
            .WithName("CreateQuestionnaire");
    }
}
