using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class UpdateQuestionnaireHandler : IUpdateQuestionnaireHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<UpdateQuestionnaireHandler> _logger;

    public UpdateQuestionnaireHandler(IQuestionnaireService service, ILogger<UpdateQuestionnaireHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(string typeId, string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Results.BadRequest("Title is required");
        }

        var questionnaireId = QuestionnaireId.Parse(typeId);
        var questionnaire = await _service.UpdateQuestionnaireAsync(
            questionnaireId,
            title,
            description);

        if (questionnaire == null)
        {
            _logger.LogWarning("Questionnaire with ID {Id} not found for update", typeId);
            return Results.NotFound();
        }

        var delta = new QuestionnaireDelta
        {
            Id = questionnaire.Id,
            Title = Patchable<string>.Set(questionnaire.Title),
            Description = Patchable<string?>.Set(questionnaire.Description)
        };

        var response = new QuestionnaireUpdatedResponse
        {
            Delta = delta
        };

        return Results.Ok(response);
    }
}
