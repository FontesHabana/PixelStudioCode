namespace MyConsole;
using System;
using System.Linq;
using Editor;

/// <summary>
/// Represents a console command to navigate to the previous file or view in the editor's history.
/// </summary>
public class Undo : IConsoleCommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public virtual string Name { get; }
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public virtual string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Undo"/> class.
    /// </summary>
    public Undo()
    {
        Name = "undo";
        Description = "Navigates to the previous file or view in the editor's history.";
    }

    /// <summary>
    /// Executes the undo command, navigating the main instance to the previous view.
    /// </summary>
    /// <param name="args">The arguments passed to the command. If any arguments are passed, an exception is thrown.</param>
    /// <param name="mainInstance">The main UI instance to navigate.</param>
    /// <exception cref="System.Exception">Thrown when unknown arguments are passed to the command.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    { if (args.Count() == 0)
        {
            mainInstance.GoBack();
        }
        else {
         throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
    }
        
    }
}