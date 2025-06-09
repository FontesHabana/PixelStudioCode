using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using Godot;


namespace MyConsole;

/// <summary>
/// Provides a static console for handling commands.
/// </summary>
public static class Console
{
    /// <summary>
    /// A dictionary containing all available console commands.
    /// </summary>
    public static readonly Dictionary<string, IConsoleCommand> commands = new()
    {
        { "clear console", new ClearConsole() },
        { "run", new Run() },
        { "run file", new RunFile() },
        { "new file", new NewFile() },
        { "help", new Help() },
        {"clear canvas", new ClearCanvas()},
        { "redo", new Redo() },
        { "undo", new Undo() },
        {"resize",new Resize()},
        {"show commands",new ShowCommands()},
         {"generate code",new GenerateCode()}
    };

    /// <summary>
    /// Handles the input received from the console.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    public static void HandleInput(string input, main_ui mainInstance)
    {
        GD.Print(input);

        try
        {
            var (commandName, args) = ParseCommand(input, commands);
            for (int i = 0; i < args.Length; i++)
            {
                args[i] = args[i].ToLower();
                GD.Print(args[i]);
            }
           
                GD.Print(commandName);
            if (string.IsNullOrEmpty(commandName))
            {
                return;
            }
            if (commands.TryGetValue(commandName, out var command))
            {
                command.Execute(args, mainInstance);
                if (mainInstance._consoleOutput.Text[mainInstance._consoleOutput.Text.Length - 1] == '>')
                {
                    return;
                }
                mainInstance._consoleOutput.ConsoleLog($"\n {mainInstance.infoConsole} \n>>>");
            }
            else
            {
                throw new SystemException($"Error: Unknown command. Type help to see a list of available commands.");
            }
            
        }
        catch (SystemException error)
        {

            mainInstance._consoleOutput.ConsoleLog($"\n{error.Message}\n {mainInstance.infoConsole} \n>>>");
        }
       
     
    }

    /// <summary>
    /// Parses the input string to extract the command name and arguments.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="commands">The dictionary of available commands.</param>
    /// <returns>A tuple containing the command name and an array of arguments.</returns>
    public static (string commandName, string[] args) ParseCommand(string input, Dictionary<string, IConsoleCommand> commands)
    {
        string[] tokens = input.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0)
        {
            return (string.Empty, Array.Empty<string>());
        }

        if (tokens.Length >= 2)
        {
            string twoWordCommand = $"{tokens[0].ToLower()} {tokens[1].ToLower()}";
        
            if (commands.ContainsKey(twoWordCommand))
            {
                return (twoWordCommand, tokens.Skip(2).ToArray());
            }
        }



        return (tokens[0], tokens.Skip(1).ToArray());

    }
}