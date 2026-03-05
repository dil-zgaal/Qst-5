using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class DeleteQuestionnaireHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<DeleteQuestionnaireHandler> _logger;

    public DeleteQuestionnaireHandler(IQuestionnaireService service, ILogger<DeleteQuestionnaireHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(string typeId)
    {
        var questionnaireId = QuestionnaireId.Parse(typeId);
        var success = await _service.DeleteQuestionnaireAsync(questionnaireId);

        if (!success)
        {
            _logger.LogWarning("Questionnaire with ID {Id} not found for deletion", typeId);
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}
