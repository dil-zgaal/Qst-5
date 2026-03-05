using Questionnaire.Handlers;

namespace Questionnaire.Requests;

public static class GetQuestionnaire
{
    public static RouteHandlerBuilder MapGetQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapGet("/{typeId}", async (string typeId, GetQuestionnaireHandler handler) =>
            await handler.HandleAsync(typeId))
            .WithName("GetQuestionnaire");
    }
}
