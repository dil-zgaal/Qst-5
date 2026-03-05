using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Questionnaires.Contract.Handlers;
using Questionnaires.Contract.Commands;
using Questionnaires.Contract.Models.Commands;
using Questionnaires.Handlers.Commands;

namespace Questionnaires.Handlers;

public static class HandlerExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        // Register command handlers
        services.AddScoped<ICommandHandler<UpdateQuestionnaireProperty>, UpdateQuestionnairePropertyHandler>();

        // Register concrete handlers with their interfaces
        services.AddScoped<IListQuestionnairesHandler, ListQuestionnairesHandler>();
        services.AddScoped<IGetQuestionnaireHandler, GetQuestionnaireHandler>();
        services.AddScoped<ICreateQuestionnaireHandler, CreateQuestionnaireHandler>();
        services.AddScoped<IUpdateQuestionnaireHandler, UpdateQuestionnaireHandler>();
        services.AddScoped<IDeleteQuestionnaireHandler, DeleteQuestionnaireHandler>();

        return services;
    }
}
