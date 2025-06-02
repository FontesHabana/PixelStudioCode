using System.Linq;
using Editor;
using System;

namespace MyConsole;
/// <summary>
/// Represents a console command to clear the canvas.
/// </summary>
public partial class ClearCanvas: IConsoleCommand
{   /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public virtual string  Name { get; }
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public virtual string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClearCanvas"/> class.
    /// </summary>
    public ClearCanvas()
    {
        Name = "clear canvas";
        Description = "Resets the drawing or visual canvas to its initial empty state.";
    }

    /// <summary>
    /// Executes the clear canvas command.
    /// </summary>
    /// <param name="args">The arguments passed to the command. If any arguments are passed, an exception is thrown.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="System.Exception">Thrown when unknown arguments are passed to the command.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance.CleanCanvas();
        }
        else
        {
        throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
    }
     
    }

}