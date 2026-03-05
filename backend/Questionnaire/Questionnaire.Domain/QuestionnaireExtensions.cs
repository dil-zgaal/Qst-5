using Questionnaire.Contract.Handlers;
using Questionnaire.Repositories;
using Questionnaire.Services;
using Questionnaire.Handlers;

namespace Questionnaire;

public static class QuestionnaireExtensions
{
    public static IServiceCollection AddQuestionnaireDomain(this IServiceCollection services)
    {
        // Repository layer
        services.AddSingleton<IQuestionnaireRepository, QuestionnaireRepository>();

        // Service layer
        services.AddScoped<IQuestionnaireService, QuestionnaireService>();

        // Handler layer - Auto-register all handlers
        services.AddHandlers();

        return services;
    }

    public static IEndpointRouteBuilder MapQuestionnaireRoutes(this IEndpointRouteBuilder app)
    {
        app.MapQuestionnaireEndpoints();
        return app;
    }
}
