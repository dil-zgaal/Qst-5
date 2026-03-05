using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Questionnaires.Requests;

public static class QuestionnaireRouter
{
    public static void MapQuestionnaireEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/questionnaires")
            .WithTags("Questionnaires");

        group.MapListQuestionnaires();
        group.MapGetQuestionnaire();
        group.MapCreateQuestionnaire();
        group.MapUpdateQuestionnaire();
        group.MapDeleteQuestionnaire();
    }
}
