using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Models;

namespace Questionnaires.Requests;

public class CreateQuestionnaireRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Question> Content { get; set; } = new();
}

public static class CreateQuestionnaire
{
    public static RouteHandlerBuilder MapCreateQuestionnaire(this RouteGroupBuilder group)
    {
        return group.MapPost("/", async (CreateQuestionnaireRequest request, ICreateQuestionnaireHandler handler) =>
        {
            try
            {
                var delta = await handler.HandleAsync(request.Title, request.Description, request.Content);

                var response = new QuestionnaireUpdatedResponse
                {
                    Delta = delta
                };

                return Results.Created($"/questionnaires/{delta.Id}", response);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .WithName("CreateQuestionnaire");
    }
}
