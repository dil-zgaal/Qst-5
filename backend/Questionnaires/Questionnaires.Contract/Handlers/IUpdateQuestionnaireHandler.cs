using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Handlers;

public interface IUpdateQuestionnaireHandler
{
    Task<QuestionnaireDelta?> HandleAsync(QuestionnaireId id, long fromVersion, UpdateCommand command);
}
