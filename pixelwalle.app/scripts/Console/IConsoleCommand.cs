namespace MyConsole;

using Editor;
/// <summary>
/// Defines the interface for console commands.
/// </summary>
public interface IConsoleCommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    string Description { get; }
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="args">The arguments passed to the command.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    void Execute(string[] args, main_ui mainInstance);
}