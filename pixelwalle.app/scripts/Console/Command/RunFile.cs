namespace MyConsole;

using System.Linq;
using Editor;
using System.IO;
using System;
/// <summary>
/// Represents a console command to run a specified file.
/// </summary>
class RunFile: IConsoleCommand
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
    /// Initializes a new instance of the <see cref="RunFile"/> class.
    /// </summary>
    public RunFile()
    {
        Name = "run file";
        Description = "	Runs the selected file independently, regardless of the currently focused tab.";
    }

    /// <summary>
    /// Executes the command to run a file.
    /// </summary>
    /// <param name="args">The arguments passed to the command. It expects one argument: the file path.</param>
    /// <param name="mainInstance">The main UI instance.</param>
    /// <exception cref="System.Exception">Thrown when the file path is invalid or the file does not end with '.pw'.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {
        if (args.Count() == 1)
        {
            string path = args[0];
            string fileContent = "";
            if (!path.EndsWith(".pw"))
            {
                throw new System.Exception("direccion no valida");
                // _consoleOutput.Text += "\n" + "Solo se admiten archivos terminados en .pw" + "\n" + userScript + "\n" + ">>>";

            }
            try
            {
                 fileContent = File.ReadAllText(path);
            }
            catch (SystemException)
            {

                throw new SystemException($"Error: Invalid path or address {path}. Please check the location and try again.");
            }


            mainInstance.OnPlayPressedArgs(fileContent);
        }
        else
        {
         throw new SystemException($"Error: Unknown command {Name}. Type help to see a list of available commands.");
        }
       
    }
}