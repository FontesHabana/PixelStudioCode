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
        Description = @"
🧠 PixelWalle Language - Command Reference Guide

Below is a list of valid syntax elements and built-in commands you can use when writing code in the PixelWalle language.

──────────────────────────────────────
📦 Variable Declarations
──────────────────────────────────────
name <- expression
    Declares a variable and assigns the result of an expression.

──────────────────────────────────────
📚 List Declarations & Access
──────────────────────────────────────
name <- List<type>[values]
    Declares a typed list and initializes it with values.

name[index]
    Accesses the element at the specified index in the list.

──────────────────────────────────────
🧩 Built-in Functions
──────────────────────────────────────
name.Count
    Returns the number of elements in the list.

GetActualX()
    Gets the current brush X position.

GetActualY()
    Gets the current brush Y position.

GetCanvasSize()
    Returns the canvas width.

GetColorCount(color, x1, y1, x2, y2)
    Returns how many times a specific color appears in a defined rectangular area.

IsBrushColor(color)
    Checks if the current brush color matches the given color.

IsBrushSize(size)
    Checks if the brush size matches the given value.

IsCanvasColor(color, x, y)
    Checks if the pixel at (x, y) has the specified color.

──────────────────────────────────────
📜 List Commands
──────────────────────────────────────
name.Add(value)
    Adds a value to the end of the list.

name.Clear()
    Removes all elements from the list.

name.RemoveAt(index)
    Removes the element at the specified index.

──────────────────────────────────────
🎨 Drawing & Canvas Commands
──────────────────────────────────────
Size(size)
    Sets the brush size.
Color(color)
    Sets the brush color.

DrawCircle(x, y, radius)
    Draws a circle after move  radius position in direction (x, y) with the given radius.

DrawLine(dirX, dirY, length)
    Draws a straight line from the current position in the specified direction.

DrawRectangle(dirX, dirY, length, width, height)
    Draws a rectangle starting from current position and using the given dimensions.

Fill()
    Fills the canvas with the current brush color.

──────────────────────────────────────
🔀 Control Flow & Utility Commands
──────────────────────────────────────
GoTo[label](condition)
    Jumps to a labeled section in the code if the condition is true.

Print(expression)
    Displays the value of an expression in the console.

ReSpawn(x, y)
    Resets the brush to the given position.

Spawn(x, y)
    Sets the initial brush position on the canvas.

Run(filepath)
    Loads and executes code from the specified file.

──────────────────────────────────────
🧭 Tip:
Use 'help [command]' for detailed information about any specific instruction.
──────────────────────────────────────
";
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