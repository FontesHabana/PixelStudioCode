namespace MyConsole;

using Editor;
using System;
using System.Linq;

   
/// <summary>
/// A console command that displays available commands.
/// </summary>
class ShowCommands: IConsoleCommand
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
    /// Initializes a new instance of the <see cref="ShowCommands"/> class.
    /// </summary>
    public ShowCommands()
    {
        Name = "show commands";
        Description = "Show PixellWallE commands documentation";
  }

  
    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="args">The arguments passed to the command.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="SystemException">Thrown when unexpected arguments are provided.</exception>
      public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance._consoleOutput.ConsoleLog("\n"+Description);

        }
        else
        {
            throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
        }
       
    }
}