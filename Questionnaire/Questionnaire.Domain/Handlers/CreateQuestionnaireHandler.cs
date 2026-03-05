using Questionnaire.Models;
using Questionnaire.Requests;
using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class CreateQuestionnaireHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<CreateQuestionnaireHandler> _logger;

    public CreateQuestionnaireHandler(IQuestionnaireService service, ILogger<CreateQuestionnaireHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(CreateQuestionnaireRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Results.BadRequest("Title is required");
        }

        var questionnaire = await _service.CreateQuestionnaireAsync(request.Title, request.Description, request.Content);

        var delta = new QuestionnaireDelta
        {
            Id = questionnaire.Id,
            Title = Patchable<string>.Set(questionnaire.Title),
            Description = Patchable<string?>.Set(questionnaire.Description),
            CreatedAt = Patchable<DateTime>.Set(questionnaire.CreatedAt)
        };

        var response = new QuestionnaireUpdatedResponse
        {
            Delta = delta
        };

        return Results.Created($"/questionnaires/{questionnaire.Id}", response);
    }
}
