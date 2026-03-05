using Questionnaire.Handlers;

namespace Questionnaire.Requests;

public class UpdateQuestionnaireRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}

public static class UpdateQuestionnaire
{
    public static RouteHandlerBuilder MapUpdateQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapPatch("/{typeId}", async (string typeId, UpdateQuestionnaireRequest request, UpdateQuestionnaireHandler handler) =>
            await handler.HandleAsync(typeId, request))
            .WithName("UpdateQuestionnaire");
    }
}
