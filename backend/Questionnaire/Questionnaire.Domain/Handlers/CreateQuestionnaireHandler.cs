using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class CreateQuestionnaireHandler : ICreateQuestionnaireHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<CreateQuestionnaireHandler> _logger;

    public CreateQuestionnaireHandler(IQuestionnaireService service, ILogger<CreateQuestionnaireHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(string title, string? description, List<Question> content)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Results.BadRequest("Title is required");
        }

        var questionnaire = await _service.CreateQuestionnaireAsync(title, description, content);

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
