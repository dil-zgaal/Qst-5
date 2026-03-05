using Questionnaires;

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

// Add Questionnaire Domain
builder.Services.AddQuestionnaireDomain();

var app = builder.Build();

// Logging
app.Logger.LogInformation("Questionnaire API starting...");

// Map Questionnaire routes
app.MapQuestionnaireRoutes();

app.Run();
