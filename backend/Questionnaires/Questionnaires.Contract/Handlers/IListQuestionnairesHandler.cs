using Questionnaires.Contract.Models;

namespace Questionnaires.Contract.Handlers;

public interface IListQuestionnairesHandler
{
    Task<IReadOnlyList<QuestionnaireMeta>> HandleAsync();
}
