using Questionnaire.Handlers;

namespace Questionnaire.Requests;

public static class ListQuestionnaires
{
    public static RouteHandlerBuilder MapListQuestionnaires(this RouteGroupBuilder group)
    {
        return group.MapGet("/", async (ListQuestionnairesHandler handler) =>
            await handler.HandleAsync())
            .WithName("ListQuestionnaires");
    }
}
