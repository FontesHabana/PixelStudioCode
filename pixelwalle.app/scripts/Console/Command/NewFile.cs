namespace MyConsole;

using Editor;
using System;
using System.Linq;

class NewFile:IConsoleCommand
{
    
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public virtual string  Name { get; }
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public virtual string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NewFile"/> class.
    /// </summary>
    public NewFile()
    {
        Name = "new file";
        Description = "Creates a new blank file in the current workspace.";
    }

    /// <summary>
    /// Executes the new file command.
    /// </summary>
    /// <param name="args">The arguments passed to the command. If any arguments are passed, an exception is thrown.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="System.Exception">Thrown when unknown arguments are passed to the command.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance.NewFile();
        }
        else
        {
         throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
    }
        
    }
}