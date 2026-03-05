using Questionnaire.Contract.Handlers;

namespace Questionnaire.Requests;

public static class ListQuestionnaires
{
    public static RouteHandlerBuilder MapListQuestionnaires(this RouteGroupBuilder group)
    {
        return group.MapGet("/", async (IListQuestionnairesHandler handler) =>
            await handler.HandleAsync())
            .WithName("ListQuestionnaires");
    }
}
