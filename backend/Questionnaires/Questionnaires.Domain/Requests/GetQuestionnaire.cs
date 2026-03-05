using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Contract.Handlers;

namespace Questionnaires.Requests;

public static class GetQuestionnaire
{
    public static RouteHandlerBuilder MapGetQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapGet("/{id}", async ([FromRoute] QuestionnaireId id, IGetQuestionnaireHandler handler) =>
        {
            var questionnaire = await handler.HandleAsync(id);

            if (questionnaire == null)
            {
                return Results.NotFound();
            }

            var response = new QuestionnaireResponse
            {
                Questionnaire = questionnaire
            };

            return Results.Ok(response);
        })
        .WithName("GetQuestionnaire");
    }
}
