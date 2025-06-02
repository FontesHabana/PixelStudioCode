using System;
using System.Collections.Generic;
using System.Linq;
using Editor;
using Godot;


namespace MyConsole;


public static class Console
{
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
         {"resize",new Resize()}
    };

    public static void HandleInput(string input, main_ui mainInstance)
    {
        GD.Print(input);

        try
        {
            var (commandName, args) = ParseCommand(input, commands);
            if (string.IsNullOrEmpty(commandName))
                return;
            if (commands.TryGetValue(commandName, out var command))
            {
                command.Execute(args, mainInstance);
            }
            else
            {
                throw new SystemException();
            }
        }
        catch (System.Exception error)
        {

            GD.Print(error.ToString());
        }
       
     
    }


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