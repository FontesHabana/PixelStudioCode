namespace MyConsole;

using System;
using System.Linq;
using Editor;
/// <summary>
/// Represents a console command to resize the canvas or output window.
/// </summary>
class Resize: IConsoleCommand
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
    /// Initializes a new instance of the <see cref="Resize"/> class.
    /// </summary>
    public Resize()
    {
        Name = "resize";
        Description = "Resizes the canvas or output window to fit the desired dimensions or scale.";
    }

    /// <summary>
    /// Executes the resize command.
    /// </summary>
    /// <param name="args">The arguments passed to the command. It expects one argument: the desired dimensions or scale.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="System.Exception">Thrown when an incorrect number of arguments are passed to the command.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 1)
        {
            mainInstance.ResizedCanvas(args[0]);
        }
        else
        {
            throw new SystemException($"Error: Unknown command {Name}. Type help to see a list of available commands.");
        }
      
    }
}