using Questionnaires.Contract.Models;

namespace Questionnaires.Contract.Handlers;

public interface ICreateQuestionnaireHandler
{
    Task<QuestionnaireDelta> HandleAsync(string title, string? description, List<Question> content);
}
