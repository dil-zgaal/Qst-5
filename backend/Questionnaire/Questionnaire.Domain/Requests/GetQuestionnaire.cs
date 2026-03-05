using Questionnaire.Contract.Handlers;

namespace Questionnaire.Requests;

public static class GetQuestionnaire
{
    public static RouteHandlerBuilder MapGetQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapGet("/{typeId}", async (string typeId, IGetQuestionnaireHandler handler) =>
            await handler.HandleAsync(typeId))
            .WithName("GetQuestionnaire");
    }
}
