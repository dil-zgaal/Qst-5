using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Contract.Handlers;

namespace Questionnaires.Requests;

public static class ListQuestionnaires
{
    public static RouteHandlerBuilder MapListQuestionnaires(this RouteGroupBuilder group)
    {
        return group.MapGet("/", async (IListQuestionnairesHandler handler) =>
        {
            var questionnaires = await handler.HandleAsync();

            var response = new QuestionnaireListResponse
            {
                Questionnaires = questionnaires.Select(q => new QuestionnaireListItemResponse
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    CreatedAt = q.CreatedAt
                }).ToList()
            };

            return Results.Ok(response);
        })
        .WithName("ListQuestionnaires");
    }
}
