namespace Questionnaires.Contract.Models.Commands;

/// <summary>
/// Command to update questionnaire properties
/// </summary>
public class UpdateQuestionnaireProperty : UpdateCommand
{
    public const string CommandType = "updateProperty";
    public override string Type => CommandType;

    public string? Title { get; init; }
    public string? Description { get; init; }
}
