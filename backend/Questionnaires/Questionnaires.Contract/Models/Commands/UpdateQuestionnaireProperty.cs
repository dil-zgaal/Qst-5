using Core.Model.Delta;

namespace Questionnaires.Contract.Models.Commands;

/// <summary>
/// Command to update questionnaire properties
/// </summary>
public class UpdateQuestionnaireProperty : UpdateCommand
{
    public const string CommandType = "updateProperty";
    public override string Type => CommandType;

    public required string Title { get; init; }
    public required string? Description { get; init; }
}
