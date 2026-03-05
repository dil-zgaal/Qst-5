using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Contract.Handlers;

namespace Questionnaires.Requests;

public static class DeleteQuestionnaire
{
    public static RouteHandlerBuilder MapDeleteQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapDelete("/{id}", async ([FromRoute] QuestionnaireId id, IDeleteQuestionnaireHandler handler) =>
        {
            var success = await handler.HandleAsync(id);

            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .WithName("DeleteQuestionnaire");
    }
}
