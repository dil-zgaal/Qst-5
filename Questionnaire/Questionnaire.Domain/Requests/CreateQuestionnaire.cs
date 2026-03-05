using Questionnaire.Handlers;
using Questionnaire.Models;

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
        return group.MapPost("/", async (CreateQuestionnaireRequest request, CreateQuestionnaireHandler handler) =>
            await handler.HandleAsync(request))
            .WithName("CreateQuestionnaire");
    }
}
