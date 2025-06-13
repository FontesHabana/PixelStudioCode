using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using PixelWallE.Language;
using PixelWallE.Core;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using MyConsole;
namespace Editor;

public partial class main_ui : Control
{
    // Member variables to store references to the nodes
    [Export] CodeEdit _codeEditNode;
    [Export] ColorRect _errorTooltip;

    [Export] public TextEdit _consoleOutput;
    [Export] Button _cleanConsole;

    [Export] CanvasController _canvas;

    [Export] Button _docs;
    [Export] public FileDialog _pictureFileDialog;
    [Export] FileDialog _saveFileDialog;
    [Export] FileDialog _loadFileDialog;
    [Export] FileDialog _savePictureDialog;
    [Export] MenuButton _controlArchives;
    [Export] Button _closeButton;

    [Export] Button _resizeButton;
    [Export] LineEdit _resizeInput;
    [Export] Button _cleanCanvas;
    [Export] Button _processButton;

    [Export] Button _goBackButton;
    [Export] Button _goNextButton;

    [Export] RichTextLabel _lineInfo;

    private bool isRunning = false;
    private CancellationTokenSource cancellationTokenSource;
    private static Canvas canvas = new Canvas(25);
    private static Stack<Canvas> stackGoBack;
    private static Stack<Canvas> stackGoNext;
    public static Interpreter interpreter = new Interpreter(canvas, ""); // Initialize the interpreter with a default size
    private string userScript => $" {DateTime.Now.ToString()} 🤖";
    private string? filePath = null;
    public string? picturePath = null;

    public string? infoConsole => string.IsNullOrEmpty(filePath) ? $"*** New File {userScript} ***" : $"*** {filePath} {userScript} ***";

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Initializes the console output, and the navigation stacks.
    /// </summary>
    public override void _Ready()
    {
        GD.Print("--> Método _Ready() iniciado.");
        _consoleOutput.Text = infoConsole;
        _consoleOutput.AppendPrompt();
        stackGoBack = new Stack<Canvas>();
        stackGoNext = new Stack<Canvas>();

        GD.Print("--> Método _Ready() finalizado.");
    }

    #region Button Handlers

    //-----------------------------------TopRegionButtons-----------------------------------------------
    /// <summary>
    /// Handles the event when a menu item is pressed in the control archives.
    /// </summary>
    /// <param name="id">The ID of the menu item pressed.</param>
    private void OnMenuItemPressed(long id)
    {
        switch (id)
        {
            case 0:
                _loadFileDialog.Popup();
                break;
            case 1:
                _saveFileDialog.Popup();
                break;
            case 2:
                _savePictureDialog.Popup();
                break;
        }
    }
    /// <summary>
    /// Connects the IdPressed signal of the control archives popup to the OnMenuItemPressed method.
    /// </summary>
    private void PressedArchiveControl()
    {
        _controlArchives.GetPopup().IdPressed += OnMenuItemPressed;
    }
    /// <summary>
    /// Opens the documentation in the default web browser.
    /// </summary>
    private void DocsPressed()
    {
        OS.ShellOpen("https://fonteshabana.github.io/Pixel_WallE_docs/");
    }
    /// <summary>
    /// Closes the application.
    /// </summary>
    private void ClosePressed()
    {
        GetTree().Quit();
    }
    //------------------------------------------------------------------------------------------------

    //-----------------------------------LeftRegionBUttons-----------------------------------------------
    /// <summary>
    /// Handles the button press event, initiating the code execution process.
    /// </summary>
    private async void OnButtonPressed()
    {
        try
        {
            await OnPlayPressedArgsAsync();
        }
        catch (System.Exception error)
        {
            GD.Print(error);
            // Restore UI in case of error
            SetUIRunningState(false);
        }
        _canvas.QueueRedraw();
    }

    /// <summary>
    /// Executes the code either from the code edit node or from an alternative code string.
    /// </summary>
    /// <param name="alternativeCode">An optional string containing alternative code to execute.</param>
    public async Task OnPlayPressedArgsAsync(string alternativeCode = null)
    {
        if (isRunning)
        {
            // If it's already running, cancel the current operation
            CancelCurrentExecution();
            return;
        }

        string codeFromEdit = _codeEditNode.Text;
        if (alternativeCode != null)
        {
            codeFromEdit = alternativeCode;
        }

        await ProcessCodeAsync(codeFromEdit);
        _goBackButton.Visible = true;
    }

    /// <summary>
    /// Cancels the current code execution, if one is in progress.
    /// </summary>
    private void CancelCurrentExecution()
    {
        if (cancellationTokenSource != null && !cancellationTokenSource.Token.IsCancellationRequested)
        {
            cancellationTokenSource.Cancel();
            _consoleOutput.ConsoleLog("⚠️ Execution cancelled by the user\n");
            SetUIRunningState(false);

        }
    }

    /// <summary>
    /// Sets the UI state based on whether the code is currently running.
    /// </summary>
    /// <param name="running">True if the code is running, false otherwise.</param>
    private void SetUIRunningState(bool running)
    {
        isRunning = running;

        // Change button text according to the state
        _processButton.Text = running ? "Cancelar" : "";
        _processButton.Modulate = running ? new Godot.Color(1, 0.5f, 0.5f) : new Godot.Color(1, 1, 1);

        // Optional: disable other buttons while running
        _cleanCanvas.Disabled = running;
        _resizeButton.Disabled = running;
        _goBackButton.Disabled = running;
        _goNextButton.Disabled = running;
        _consoleOutput.Editable = !running;
    }

    /// <summary>
    /// Handles the resize button press, toggling the visibility of the resize input or resizing the canvas.
    /// </summary>
    private void ResizePressed()
    {
        if (!_resizeInput.Visible)
        {
            _resizeInput.Visible = true;
            return;
        }

        ResizedCanvas(_resizeInput.Text);
    }

    /// <summary>
    /// Cleans the canvas by creating a new empty canvas and updating the interpreter.
    /// </summary>
    public void CleanCanvas()
    {
        Canvas current = new Canvas(canvas.Size);
        CopyMatrix(interpreter.Canvas, current);
        stackGoBack.Push(current);
        canvas = new Canvas(canvas.Size);
        interpreter.Canvas = canvas;
        _canvas._Ready();
        _canvas.QueueRedraw();
    }

    /// <summary>
    /// Navigates back to the previous canvas state, if available.
    /// </summary>
    public void GoBack()
    {
        if (stackGoBack.Count > 0)
        {
            Canvas current = new Canvas(canvas.Size);
            CopyMatrix(interpreter.Canvas, current);
            stackGoNext.Push(current);
            canvas = stackGoBack.Pop();
            interpreter.Canvas = canvas;
            
            _canvas.QueueRedraw();
        }
        else
        {
            _goBackButton.Visible = false;
        }
        _goNextButton.Visible = true;
    }

    /// <summary>
    /// Navigates forward to the next canvas state, if available.
    /// </summary>
    public void GoNext()
    {
        if (stackGoNext.Count > 0)
        {
            Canvas current = new Canvas(canvas.Size);
            CopyMatrix(interpreter.Canvas, current);
            stackGoBack.Push(current);
            interpreter.Canvas = stackGoNext.Pop();
            canvas = interpreter.Canvas;
            _canvas.QueueRedraw();
            _goBackButton.Visible = true;
        }
        else
        {
            _goNextButton.Visible = false;
        }
    }
    //------------------------------------------------------------------------------------------------

    /// <summary>
    /// Cleans the console output.
    /// </summary>
    public void CleanConsole()
    {
        _consoleOutput.Clear();
        _consoleOutput.ConsoleLog($"{infoConsole} \n>>>");
    }

    #endregion

    #region ExecutionManager
    //------------------------------------Execution Manager-------------------------------------------

    /// <summary>
    /// Processes the code asynchronously, executing it and updating the UI with the results.
    /// </summary>
    /// <param name="codeString">The code to be executed.</param>
    private async Task ProcessCodeAsync(string codeString)
    {
        // Change UI to "running" state
        SetUIRunningState(true);
        _consoleOutput.ConsoleLog("🔄 Executing code...\n");

        // Create cancellation token
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = new CancellationTokenSource();

        // Prepare canvas for history
        Canvas current = new Canvas(canvas.Size);
        CopyMatrix(canvas, current);
        stackGoBack.Push(current);
        stackGoNext.Clear();
        _goBackButton.Visible = false;


        try
        {
            // Execute in a separate thread
            var result = await Task.Run(() => ExecuteCodeInBackground(codeString, cancellationTokenSource.Token),
                                      cancellationTokenSource.Token);

            // Update UI on the main thread with the results
            if (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                UpdateUIWithResults(result);
            }
        }
        catch (OperationCanceledException)
        {
            _consoleOutput.ConsoleLog("⚠️ Execution cancelled\n");
        }
        catch (Exception ex)
        {
            _consoleOutput.ConsoleLog($"❌ Error during execution: {ex.Message}\n");
            GD.PrintErr($"Error en ProcessCodeAsync: {ex}");
        }
        finally
        {
            // Restore UI
            SetUIRunningState(false);
            _consoleOutput.ConsoleLog($"\n{infoConsole}\n>>>");
            _consoleOutput.ScrollVertical = _consoleOutput.GetLineCount();
        }
    }

    /// <summary>
    /// Executes the code in a background thread, allowing cancellation.
    /// </summary>
    /// <param name="codeString">The code to execute.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An ExecutionResult containing the results of the execution.</returns>
    private ExecutionResult ExecuteCodeInBackground(string codeString, CancellationToken cancellationToken)
    {
        var result = new ExecutionResult();

        try
        {
            // Create a new interpreter for the background thread
            var backgroundInterpreter = new Interpreter(canvas, codeString);

            // Check for cancellation before executing
            cancellationToken.ThrowIfCancellationRequested();

            // Execute the code
            backgroundInterpreter.Run(cancellationToken);

            // Check for cancellation after executing
            cancellationToken.ThrowIfCancellationRequested();

            // Collect results
            result.ConsoleMessages = new List<string>(backgroundInterpreter.ConsoleMessage);
            result.Errors = new List<PixelWallEException>(backgroundInterpreter.Errors);
            result.UpdatedCanvas = backgroundInterpreter.Canvas;
            result.Success = true;
        }
        catch (OperationCanceledException)
        {
            result.Success = false;
            result.WasCancelled = true;
            throw; // Re-throw to handle at the upper level
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Errors = new List<PixelWallEException>
            {
                new RuntimeException($"Internal error: {ex.Message}", new CodeLocation())
            };
        }

        return result;
    }

    /// <summary>
    /// Updates the UI with the results of the code execution.
    /// </summary>
    /// <param name="result">The ExecutionResult containing the results of the execution.</param>
    private void UpdateUIWithResults(ExecutionResult result)
    {
        if (!result.Success || result.WasCancelled)
            return;

        // Update the main interpreter with the results
        interpreter.Canvas = result.UpdatedCanvas;

        // Display console messages
        foreach (string message in result.ConsoleMessages)
        {
            _consoleOutput.ConsoleLog($"{message}\n");
        }

        // Display errors
        foreach (PixelWallEException error in result.Errors)
        {
            _consoleOutput.ConsoleLog($"{error.Message}\n");
        }

        // Success message if no errors
        if (result.Errors.Count == 0)
        {
            _consoleOutput.Text += "\n✅ Compiled successfully\n";
        }

        // Update visual canvas
        _canvas.QueueRedraw();
    }

    /// <summary>
    /// Handles changes in the code editor, updating the interpreter and highlighting errors.
    /// </summary>
    private void CodeChange()
    {
        _resizeInput.Visible = false;
        interpreter = new Interpreter(canvas, _codeEditNode.Text);
        HighlightError(interpreter.Errors);
    }

    /// <summary>
    /// Highlights the lines in the code editor that contain errors.
    /// </summary>
    /// <param name="exceptions">A list of PixelWallEException representing the errors.</param>
    private void HighlightError(List<PixelWallEException> exceptions)
    {
        Godot.Color errorLineColor = new Godot.Color(1, 0.3f, 0.3f, 0.3f);
        Godot.Color normalLineColor = new Godot.Color(0, 0, 0, 0);

        // Clear colors
        for (int i = 0; i < _codeEditNode.GetLineCount(); i++)
        {
            _codeEditNode.SetLineBackgroundColor(i, normalLineColor);
        }

        // Highlight errors
        foreach (PixelWallEException error in interpreter.Errors)
        {
            _codeEditNode.SetLineBackgroundColor(error.Location.Line - 1, errorLineColor);
        }
    }

    /// <summary>
    /// Updates the line info and error tooltip based on the current caret position.
    /// </summary>
    private void CaretChanged()
    {
        _lineInfo.Text = $"Line: {_codeEditNode.GetCaretLine() + 1}  Column: {_codeEditNode.GetCaretColumn()}";

        Godot.Color errorLineColor = new Godot.Color(1, 0.3f, 0.3f, 0.3f);

        if (_codeEditNode.GetLineBackgroundColor(_codeEditNode.GetCaretLine()) == errorLineColor)
        {
            _errorTooltip.Visible = true;
            if (_codeEditNode.GetCaretDrawPos().Y < 200)
                _errorTooltip.Position = new Vector2(60, 10 + _codeEditNode.GetCaretDrawPos().Y);
            else
                _errorTooltip.Position = new Vector2(60, _codeEditNode.GetCaretDrawPos().Y - 150);

            string message = "Unknown Message";

            for (int i = interpreter.Errors.Count - 1; i >= 0; i--)
            {
                if (interpreter.Errors[i].Location.Line - 1 == _codeEditNode.GetCaretLine())
                {
                    if (_codeEditNode.GetCaretColumn() >= interpreter.Errors[i].Location.Column)
                    {
                        message = interpreter.Errors[i].Message;
                        break;
                    }
                    else
                    {
                        message = interpreter.Errors[i].Message;
                    }
                }
            }

            Godot.TextEdit label = (Godot.TextEdit)_errorTooltip.GetChild(0);
            label.Text = message;
            label.Editable = false;
        }
        else
        {
            _errorTooltip.Visible = false;
        }
    }

    //------------------------------------------------------------------------------------------------
    #endregion
    # region Canvas Manager

    //-----------------------------------Canvas Manager ---------------------------------------------
    /// <summary>
    /// Resizes the canvas based on the input text.
    /// </summary>
    /// <param name="text">The text containing the new size for the canvas.</param>
    public void ResizedCanvas(string text)
    {
        int number;
        if (int.TryParse(text, out number))
        {
            if (number <= 1024 && number > 0)
            {
                canvas = new Canvas(number);
                interpreter.Canvas = canvas;

                _canvas._Ready();
                _canvas.QueueRedraw();
                _resizeInput.Text = "";
                _resizeInput.PlaceholderText = canvas.Size.ToString();

                stackGoBack.Clear();
                stackGoNext.Clear();
                Canvas current = new Canvas(canvas.Size);
                CopyMatrix(interpreter.Canvas, current);
                stackGoBack.Push(current);

                _goBackButton.Visible = false;
                _goNextButton.Visible = false;
            }
        }

        _resizeInput.Visible = false;
    }

    /// <summary>
    /// Closes the resize input.
    /// </summary>
    private void CloseResize()
    {
        _resizeInput.Visible = false;
    }

    /// <summary>
    /// Copies the matrix from one canvas to another.
    /// </summary>
    /// <param name="canvas">The source canvas.</param>
    /// <param name="copy">The destination canvas.</param>
    private static void CopyMatrix(Canvas canvas, Canvas copy)
    {
        for (int i = 0; i < canvas.Size; i++)
        {
            for (int j = 0; j < canvas.Size; j++)
            {
                copy.Matrix[i, j] = canvas.Matrix[i, j];
            }
        }
    }

    //------------------------------------------------------------------------------------------------
    #endregion
    #region File Manager
    //-----------------------------------File Manager ---------------------------------------------

    /// <summary>
    /// Handles the file open operation.
    /// </summary>
    /// <param name="path">The path of the file to open.</param>
    private void OnFileOpenSelected(string path)
    {
        if (!path.EndsWith(".pw"))
        {
            _consoleOutput.Text += "\n" + "Only files with the .pw extension are supported" + "\n" + userScript + "\n" + ">>>";
            return;
        }

        string fileContent = File.ReadAllText(path);
        _codeEditNode.Text = fileContent;
        filePath = path;
        CodeChange();
        _consoleOutput.ConsoleLog($"\n{infoConsole}");
        _consoleOutput.AppendPrompt();
    }

    /// <summary>
    /// Handles the file save operation.
    /// </summary>
    /// <param name="path">The path to save the file to.</param>
    private void OnFileSaveSelected(string path)
    {
        try
        {
            File.WriteAllText(path, _codeEditNode.Text);
            filePath = path;
        }
        catch (System.Exception error)
        {
            _consoleOutput.Text += "\n" + "Error occurred while saving the file" + error + "\n" + userScript + "\n" + ">>>";
        }
    }


    /// <summary>
    /// Handles the picture save operation.
    /// </summary>
    /// <param name="path">The path to save the file to.</param>
    public void OnFileSavePictureSelected(string path)
    {

        try
        {
           SaveDrawing(_canvas, path);
        }
        catch (System.Exception error)
        {
            _consoleOutput.Text += "\n" + "Error occurred while saving the file" + error + "\n" + userScript + "\n" + ">>>";
        }
        
    }

    
    /// <summary>
    /// Creates a new file, clearing the code editor and resetting the file path.
    /// </summary>
    public void NewFile()
    {
        _codeEditNode.Text = "";
        filePath = null;


    }

    /// <summary>
    /// Handles input events, specifically the "SaveCode" action.
    /// </summary>
    /// <param name="event">The input event.</param>
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("SaveCode"))
        {
            if (filePath == null)
            {
                _saveFileDialog.Popup();
            }
            else
            {
                File.WriteAllText(filePath, _codeEditNode.Text);
            }
        }
    }


    /// <summary>
    /// Try to save a picture of a texture rect.
    /// </summary>
    /// <param name="path">The path to save the file to.</param>
    public void SaveDrawing(TextureRect texturerect, string path)
    {
        if (texturerect == null)
        {
            return;
        }

        Image fullscreenshot = texturerect.GetViewport().GetTexture().GetImage();
        if (fullscreenshot == null)
        {
            GD.Print("Invalid screenshot");
        }


        Vector2 globalPos = texturerect.GlobalPosition;
        Vector2 size = texturerect.Size;

        Rect2I cropRect = new Rect2I(
            (int)globalPos.X,
            (int)globalPos.Y,
            (int)size.X,
            (int)size.Y
        );

        Vector2I screenSize = fullscreenshot.GetSize();
        cropRect = cropRect.Intersection(new Rect2I(0, 0, screenSize.X, screenSize.Y));

        Image croppedImage = fullscreenshot.GetRegion(cropRect);



        Error result = croppedImage.SavePng(path);
        if (result == Error.Ok)
        {
            GD.Print("Save Ok");
        }
        else
        {
            GD.Print("Save error");
        }



    }



    /// <summary>
    /// Cleans up resources when the node exits the scene tree.
    /// </summary>
    public override void _ExitTree()
    {
        cancellationTokenSource?.Dispose();
        base._ExitTree();
    }




    //------------------------------------------------------------------------------------------------
    #endregion
}

/// <summary>
/// Helper class to manage execution results.
/// </summary>
public class ExecutionResult
{
    public bool Success { get; set; }
    public bool WasCancelled { get; set; }
    public List<string> ConsoleMessages { get; set; } = new List<string>();
    public List<PixelWallEException> Errors { get; set; } = new List<PixelWallEException>();
    public Canvas UpdatedCanvas { get; set; }
}