using Questionnaire.Handlers;

namespace Questionnaire.Requests;

public static class DeleteQuestionnaire
{
    public static RouteHandlerBuilder MapDeleteQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{typeId}", async (string typeId, DeleteQuestionnaireHandler handler) =>
            await handler.HandleAsync(typeId))
            .WithName("DeleteQuestionnaire");
    }
}
