using System.Linq;
using Editor;
using System;
using System.Diagnostics;
using System.IO;
namespace MyConsole;
/// <summary>
/// Represents a console command to generate code for drawing a picture on the Pixel Wall-E.
/// </summary>
public partial class GenerateCode : IConsoleCommand
{   /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public virtual string Name { get; }
    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public virtual string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenerateCode"/> class.
    /// </summary>
    public GenerateCode()
    {
        Name = "generate code";
        Description = "Generates code to draw a picture on the Pixel Wall-E display based on a selected image.";
    }

    /// <summary>
    /// Executes the command to generate code for drawing a picture.
    /// </summary>
    /// <param name="args">The arguments passed to the command. Expects a single argument: the desired file name for the generated code.</param>
    /// <param name="mainInstance">The main UI instance to interact with the console and file dialog.</param>
    /// <exception cref="System.Exception">Thrown when an unexpected number of arguments is provided.</exception>
    public virtual void Execute(string[] args, main_ui mainInstance)
    {     mainInstance._consoleOutput.ConsoleLog(args.Length.ToString()+"\n");
            foreach (var item in args)
            {
            mainInstance._consoleOutput.ConsoleLog(item);
            }
        if (args.Count() == 1)
        {
            mainInstance._pictureFileDialog.Popup();
            ExecutePythonScript(mainInstance.picturePath, args[0].ToString(), mainInstance);
        }
        else
        {
            throw new SystemException($"Error: Unexpected argument for '{Name}' command. Type 'help' for a list of available commands and their usage.");
        }

    }

    /// <summary>
    /// Executes a Python script to transform the selected image into code.
    /// </summary>
    /// <param name="path">The path to the selected image.</param>
    /// <param name="file">The desired file name for the generated code.</param>
    /// <param name="mainInstance">The main UI instance to interact with the console.</param>
    public void ExecutePythonScript(string path, string file, main_ui mainInstance)
    {
       
        
          var psi = new ProcessStartInfo
        {
            FileName = "python",
            Arguments=$"\"scripts\\CodeGeneration\\transformImg.py\"",

            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(psi))
        {
            if (process == null)
            {
              mainInstance._consoleOutput.ConsoleLog("No iniciado");
            }
            using (var sw = process.StandardInput)
            {
                sw.WriteLine(path);
                sw.WriteLine(file);
            }
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            mainInstance._consoleOutput.ConsoleLog(output);
            if (!string.IsNullOrEmpty(error))
            {
                mainInstance._consoleOutput.ConsoleLog(error);
            }
        }
    }

}