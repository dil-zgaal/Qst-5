using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class GetQuestionnaireHandler : IGetQuestionnaireHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<GetQuestionnaireHandler> _logger;

    public GetQuestionnaireHandler(IQuestionnaireService service, ILogger<GetQuestionnaireHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync(string typeId)
    {
        var questionnaireId = QuestionnaireId.Parse(typeId);
        var questionnaire = await _service.GetQuestionnaireByTypeIdAsync(questionnaireId);

        if (questionnaire == null)
        {
            _logger.LogWarning("Questionnaire with ID {Id} not found", typeId);
            return Results.NotFound();
        }

        var response = new QuestionnaireResponse
        {
            Questionnaire = questionnaire
        };

        return Results.Ok(response);
    }
}
