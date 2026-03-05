using Questionnaire.Handlers;
using Questionnaire.Repositories;
using Questionnaire.Requests;
using Questionnaire.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Dependency Injection - Register layers in order
// Repository layer
builder.Services.AddSingleton<IQuestionnaireRepository, QuestionnaireRepository>();

// Service layer
builder.Services.AddScoped<IQuestionnaireService, QuestionnaireService>();

// Handler layer - Auto-register all handlers
builder.Services.AddHandlers();

var app = builder.Build();

// Logging
app.Logger.LogInformation("Questionnaire API starting...");

// Map endpoints (Request layer)
app.MapQuestionnaireEndpoints();

app.Run();
