using Questionnaire.Contract.Handlers;

namespace Questionnaire.Requests;

public static class DeleteQuestionnaire
{
    public static RouteHandlerBuilder MapDeleteQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{typeId}", async (string typeId, IDeleteQuestionnaireHandler handler) =>
            await handler.HandleAsync(typeId))
            .WithName("DeleteQuestionnaire");
    }
}
