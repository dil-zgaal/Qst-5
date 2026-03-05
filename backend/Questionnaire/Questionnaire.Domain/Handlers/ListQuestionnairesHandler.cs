using Questionnaire.Services;

namespace Questionnaire.Handlers;

public class ListQuestionnairesHandler : IListQuestionnairesHandler
{
    private readonly IQuestionnaireService _service;
    private readonly ILogger<ListQuestionnairesHandler> _logger;

    public ListQuestionnairesHandler(IQuestionnaireService service, ILogger<ListQuestionnairesHandler> logger)
    {
        _service = service;
        _logger = logger;
    }

    public async Task<IResult> HandleAsync()
    {
        var questionnaires = await _service.GetAllQuestionnairesAsync();
        var items = questionnaires.Select(q => new QuestionnaireListItemResponse
        {
            Id = q.Id,
            Title = q.Title,
            Description = q.Description,
            CreatedAt = q.CreatedAt
        }).ToList();

        var response = new QuestionnaireListResponse
        {
            Questionnaires = items
        };

        return Results.Ok(response);
    }
}
