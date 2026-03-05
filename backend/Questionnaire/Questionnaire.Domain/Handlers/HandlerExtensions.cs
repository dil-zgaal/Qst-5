using System.Reflection;

namespace Questionnaire.Handlers;

public static class HandlerExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Register concrete handlers with their interfaces
        services.AddScoped<IListQuestionnairesHandler, ListQuestionnairesHandler>();
        services.AddScoped<IGetQuestionnaireHandler, GetQuestionnaireHandler>();
        services.AddScoped<ICreateQuestionnaireHandler, CreateQuestionnaireHandler>();
        services.AddScoped<IUpdateQuestionnaireHandler, UpdateQuestionnaireHandler>();
        services.AddScoped<IDeleteQuestionnaireHandler, DeleteQuestionnaireHandler>();

        return services;
    }
}
