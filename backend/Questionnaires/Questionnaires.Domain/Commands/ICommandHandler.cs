using Questionnaires.Contract.Models;
using Questionnaires.Contract.Models.Commands;

namespace Questionnaires.Contract.Commands;

/// <summary>
/// Generic command handler interface for questionnaire update commands
/// </summary>
/// <typeparam name="TCommand">Command type</typeparam>
public interface ICommandHandler<in TCommand>
    where TCommand : UpdateCommand
{
    Task<QuestionnaireDelta> HandleAsync(QuestionnaireId id, Questionnaire questionnaire, TCommand command);
}
