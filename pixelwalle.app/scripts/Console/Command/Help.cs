namespace MyConsole;

using Editor;
using System;
using System.Linq;

   
class Help: IConsoleCommand
{
  
    public virtual string  Name { get; }

    public virtual string Description { get; }

    public Help()
    {
        Name = "redo";
        Description = @"
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
undo         - Navigates to the previously opened file or view.
redo         - Moves forward to the next file or view in history.

──────────────────────────────────────
🎨 Visual & Interface Commands
──────────────────────────────────────
resize          - Changes the size of the canvas or output window.

──────────────────────────────────────
ℹ️  Help
──────────────────────────────────────
help            - Displays this help menu with all available commands.

──────────────────────────────────────
💡 Tip:
You can always type `help [command]` for more information about a specific command.

Example: `help run`
──────────────────────────────────────
";
    }

  
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 0)
        {
            mainInstance._consoleOutput.ConsoleLog(Description +mainInstance.infoConsole);

        }
        else if (args.Count()==1 || args.Count()==2 )
        {
            
            if (Console.commands.ContainsKey(Console.ParseCommand(string.Join(" ", args), Console.commands).commandName))
            {
                IConsoleCommand command = Console.commands[Console.ParseCommand(string.Join(" ", args), Console.commands).commandName];
                mainInstance._consoleOutput.ConsoleLog(command.Name + " " + command.Description+ mainInstance.infoConsole);
            }
        }
        else
        {
            throw new SystemException($"Error: Unknown command {Name}. Type help to see a list of available commands.");
        }
       
    }
}