namespace MyConsole;
using System;
using System.Linq;
using Editor;
/// <summary>
/// Represents a console command to execute the current code in the editor.
/// </summary>
class Run:IConsoleCommand
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
    /// Initializes a new instance of the <see cref="Run"/> class.
    /// </summary>
    public Run()
    {
        Name = "run";
        Description = "Executes the current code in the editor. If a file is selected, it will run that file.";
    }

    /// <summary>
    /// Executes the run command.
    /// </summary>
    /// <param name="args">The arguments passed to the command. If any arguments are passed, an exception is thrown.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="System.Exception">Thrown when unknown arguments are passed to the command.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance.OnPlayPressedArgs();
        }
        else
        {
         throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
    }
        
    }
}