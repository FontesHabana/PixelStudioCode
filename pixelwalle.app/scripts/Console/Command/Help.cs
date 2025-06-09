namespace MyConsole;

using Editor;
using System;
using System.Linq;

   
/// <summary>
/// The Help command provides information and usage instructions for the console commands.
/// </summary>
class Help: IConsoleCommand
{
  
    public virtual string  Name { get; }

    public virtual string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Help"/> class.
    /// </summary>
    public Help()
    {
        Name = "help";
        Description =  @"
Welcome to the Interactive Console!
This console lets you control execution, navigation, and visualization inside the editor using simple text commands.

Below is a list of available commands, grouped by category:

──────────────────────────────────────
🛠️  Essential Commands
──────────────────────────────────────
run             - Executes the current code in the editor.
run file        - Executes the selected file directly.
clear console   - Clears all output and logs from the console panel.
clear canvas    - Resets the visual canvas to its initial state.
new file        - Creates a new blank file in your workspace.

──────────────────────────────────────
🧭 Navigation Commands
──────────────────────────────────────
undo            - Navigates to the previously opened file or view.
redo            - Moves forward to the next file or view in history.

──────────────────────────────────────
🎨 Visual & Interface Commands
──────────────────────────────────────
resize          - Changes the size of the canvas or output window.
show commands   - Displays a categorized list of available commands in the console.

──────────────────────────────────────
🖼️ Pixel Wall-E Commands
──────────────────────────────────────
generate code   - Generates code to draw a picture on the Pixel Wall-E display based on a selected image.

──────────────────────────────────────
ℹ️  Help
──────────────────────────────────────
help            - Displays this help menu with all available commands.
help [command]  - Displays detailed information about a specific command.

──────────────────────────────────────
💡 Tip:
You can always type `help [command]` for more information about a specific command.

Example: `help run`
──────────────────────────────────────
";
    }

  
    /// <summary>
    /// Executes the help command, displaying either the general help menu or specific command information.
    /// </summary>
    /// <param name="args">The arguments passed to the command. If empty, displays the general help menu. If contains a command name, displays help for that command.</param>
    /// <param name="mainInstance">The main UI instance to output the help information to the console.</param>
    /// <exception cref="SystemException">Thrown when an unexpected argument is provided.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance._consoleOutput.ConsoleLog("\n"+Description);

        }
        else if (args.Count() == 1 || args.Count() == 2)
        {

            if (Console.commands.ContainsKey(Console.ParseCommand(string.Join(" ", args), Console.commands).commandName))
            {
                IConsoleCommand command = Console.commands[Console.ParseCommand(string.Join(" ", args), Console.commands).commandName];
                mainInstance._consoleOutput.ConsoleLog("\n"+command.Name + " " + command.Description);
            }
            else
            {
                 throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
            }
        }
        else
        {
            throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
        }
       
    }
}