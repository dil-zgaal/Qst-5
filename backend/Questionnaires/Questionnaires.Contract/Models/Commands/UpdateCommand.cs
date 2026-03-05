namespace Questionnaires.Contract.Models.Commands;

/// <summary>
/// Base class for update commands
/// </summary>
public abstract class UpdateCommand
{
    public abstract string Type { get; }
}
