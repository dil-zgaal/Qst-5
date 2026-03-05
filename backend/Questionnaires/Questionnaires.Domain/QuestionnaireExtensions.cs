using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Questionnaires.Repositories;
using Questionnaires.Services;
using Questionnaires.Handlers;
using Questionnaires.Requests;
using Questionnaires.Contract.Repositories;
using Questionnaires.Contract.Services;

namespace Questionnaires;

public static class QuestionnaireExtensions
{
    public static IServiceCollection AddQuestionnaireDomain(this IServiceCollection services)
    {
        // Repository layer - concurrent-safe in-memory implementation
        services.AddSingleton<IQuestionnaireRepository, InMemoryQuestionnaireRepository>();
        services.AddSingleton<IQuestionnaireCommands, InMemoryQuestionnaireCommands>();

        // Service layer
        services.AddScoped<IQuestionnaireUpdateService, QuestionnaireUpdateService>();
        services.AddScoped<IQuestionnaireDeleteService, QuestionnaireDeleteService>();

        // Handler layer
        services.AddHandlers();

        return services;
    }

    public static IEndpointRouteBuilder MapQuestionnaireRoutes(this IEndpointRouteBuilder app)
    {
        app.MapQuestionnaireEndpoints();
        return app;
    }
}
