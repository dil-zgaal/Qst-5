using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Requests;

public static class UpdateQuestionnaire
{
    public static RouteHandlerBuilder MapUpdateQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapPatch("/{id}", async Task<Results<Ok<QuestionnaireUpdatedResponse>, NotFound, BadRequest<string>>> (
            [FromRoute] QuestionnaireId id,
            [FromQuery] long version,
            [FromBody] UpdateQuestionnaireProperty command,
            IUpdateQuestionnaireHandler handler) =>
        {
            try
            {
                var delta = await handler.HandleAsync(id, version, command);

                if (delta == null)
                {
                    return TypedResults.NotFound();
                }

                var response = new QuestionnaireUpdatedResponse
                {
                    Delta = delta
                };

                return TypedResults.Ok(response);
            }
            catch (ArgumentException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
        .WithName("UpdateQuestionnaire");
    }
}
