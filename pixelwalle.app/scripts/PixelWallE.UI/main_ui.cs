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
namespace Editor;

public partial class main_ui : Control // partial es importante si adjuntas el script desde el editor
{
    // Declara variables miembro para guardar las referencias a los nodos
    [Export] CodeEdit _codeEditNode;
    [Export] ColorRect _errorTooltip;

    [Export] public TextEdit _consoleOutput;
    [Export] Button _cleanConsole;

    [Export] CanvasController _canvas;





    [Export] Button _docs;
    [Export] FileDialog _saveFileDialog;
    [Export] FileDialog _loadFileDialog;
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
    private static Canvas canvas = new Canvas(25);
    private static Stack<Canvas> stackGoBack;
    private static Stack<Canvas> stackGoNext;
    public static Interpreter interpreter = new Interpreter(canvas, ""); // Inicializa el intérprete con un tamaño de 25 (ajusta según sea necesario)

    private string userScript => $" {DateTime.Now.ToString()} 🤖";
    private string? filePath = null;

    public string? infoConsole => string.IsNullOrEmpty(filePath) ? $"*** New File {userScript} ***" : $"*** {filePath} {userScript} ***";




    // Método _Ready se llama cuando el nodo y sus hijos han entrado en el árbol de escena





    public override void _Ready()
    {
        GD.Print("--> Método _Ready() iniciado.");
        _consoleOutput.Text = infoConsole;
        _consoleOutput.AppendPrompt();
        stackGoBack = new Stack<Canvas>();
        stackGoNext = new Stack<Canvas>();

        GD.Print("--> Método _Ready() finalizado.");
    }




    // Este método recibirá el texto del CodeEdit
    //hacer que este metodo genere la matriz copia completa y asi no tener que crearla de forma manual en el  proceso




    #region Buttom


    //-----------------------------------TopRegionButtons-----------------------------------------------
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
        }
    }
    private void PressedArchiveControl()
    {
        _controlArchives.GetPopup().IdPressed += OnMenuItemPressed;
    }
    private void DocsPressed()
    {
        OS.ShellOpen("https://fonteshabana.github.io/Pixel_WallE_docs/");
    }
    private void ClosePressed()
    {
        GetTree().Quit();
    }
    //------------------------------------------------------------------------------------------------

    //-----------------------------------LeftRegionBUttons-----------------------------------------------
    private async void OnButtonPressed()
    {
      await Task.Run(() =>
        {
            OnPlayPressedArgs();
        });
         _canvas.QueueRedraw();
    }
    public void OnPlayPressedArgs(string alternativeCode = null)
    {
        
        string codeFromEdit = _codeEditNode.Text; // Obtiene el texto del CodeEdit
        if (alternativeCode != null)
        {
            codeFromEdit = alternativeCode;
        }

        ProcessCode(codeFromEdit); // Llama al método de procesamiento con el texto
        _goBackButton.Visible = true;
        
    }
    private void ResizePressed()
    {
        if (!_resizeInput.Visible)
        {
            _resizeInput.Visible = true;
            return;
        }

        ResizedCanvas(_resizeInput.Text);


    }
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
    public void GoBack()
    {

        if (stackGoBack.Count > 0)
        {



            Canvas current = new Canvas(canvas.Size);
            CopyMatrix(interpreter.Canvas, current);
            stackGoNext.Push(current);
            interpreter.Canvas = stackGoBack.Pop();


            _canvas.QueueRedraw();
        }
        else
        {
            _goBackButton.Visible = false;
        }
        _goNextButton.Visible = true;


    }
    public void GoNext()
    {

        if (stackGoNext.Count > 0)
        {
            Canvas current = new Canvas(canvas.Size);
            CopyMatrix(interpreter.Canvas, current);
            stackGoBack.Push(current);
            interpreter.Canvas = stackGoNext.Pop();
            _canvas.QueueRedraw();
            _goBackButton.Visible = true;
        }
        else
        {
            _goNextButton.Visible = false;
        }

    }
    //------------------------------------------------------------------------------------------------
    public void CleanConsole()
    {
        _consoleOutput.Clear();
        _consoleOutput.ConsoleLog($"{infoConsole} \n>>>");
       // _consoleOutput.AppendPrompt();

    }


    #endregion



    //------------------------------------Executer Manager-------------------------------------------

    private void ProcessCode(string codeString)
    {


   Canvas current = new Canvas(canvas.Size);
        CopyMatrix(canvas, current);
        stackGoBack.Push(current);
        interpreter = new Interpreter(canvas, codeString);

        interpreter.Run();

        foreach (string message in interpreter.ConsoleMessage)
        {
            _consoleOutput.ConsoleLog($"{message} \n");

        }
        foreach (PixelWallEException error in interpreter.Errors)
        {
            _consoleOutput.ConsoleLog($"{error.Message} \n");

        }
        
        if (interpreter.Errors.Count == 0)
        {
            _consoleOutput.Text += "\n Compilado correctamente \n";
        }

        _consoleOutput.ConsoleLog("\n" + infoConsole + "\n" + ">>>");

        _consoleOutput.ScrollVertical = _consoleOutput.GetLineCount();
        _canvas.QueueRedraw();


        // Si quisieras añadir líneas nuevas en lugar de reemplazar, podrías hacer:
        // _consoleOutput.Text += "Texto recibido: " + codeString + "\n";


        // await ToSignal(GetTree(), "process_frame");

    }

    private void CodeChange()
    {
        
             _resizeInput.Visible = false;
        interpreter = new Interpreter(canvas, _codeEditNode.Text);
        HighlightError(interpreter.Errors);
        // _codeEditNode.Text.ShowErrors(interpreter.Errors);
       
       
    }
    private void HighlightError(List<PixelWallEException> exceptions)
    {
        Godot.Color errorLineColor = new Godot.Color(1, 0.3f, 0.3f, 0.3f); // Rojo suave
        Godot.Color normalLineColor = new Godot.Color(0, 0, 0, 0); // Transparente = fondo normal

        // Limpiamos todos los colores antes de resaltar errores nuevos
        for (int i = 0; i < _codeEditNode.GetLineCount(); i++)
        {
            _codeEditNode.SetLineBackgroundColor(i, normalLineColor);
        }

        // Verifica errores línea por línea (aquí puedes meter tu lógica personalizada de PixelWalle)
        foreach (PixelWallEException error in interpreter.Errors)
        {
            _codeEditNode.SetLineBackgroundColor(error.Location.Line - 1, errorLineColor);
        }
    }
    private void CaretChanged()
    {

        _lineInfo.Text = $"Line: {_codeEditNode.GetCaretLine()}  Column: {_codeEditNode.GetCaretColumn()}";

        Godot.Color errorLineColor = new Godot.Color(1, 0.3f, 0.3f, 0.3f); // Rojo suave
        GD.Print(_codeEditNode.GetLineBackgroundColor(_codeEditNode.GetCaretLine()));
        if (_codeEditNode.GetLineBackgroundColor(_codeEditNode.GetCaretLine()) == errorLineColor)
        {
            _errorTooltip.Visible = true;
            if (_codeEditNode.GetCaretDrawPos().Y < 200)
                _errorTooltip.Position = new Vector2(60, 10 + _codeEditNode.GetCaretDrawPos().Y);
            else
                _errorTooltip.Position = new Vector2(60, _codeEditNode.GetCaretDrawPos().Y - 110);

            string message = "Mensaje desconocido";
            foreach (var item in interpreter.Errors)
            {
                if (item.Location.Line - 1 == _codeEditNode.GetCaretLine())
                {
                    message = item.Message;
                }
            }
            Godot.TextEdit label = (Godot.TextEdit)_errorTooltip.GetChild(0);
            GD.Print("guardado valor con exito");
            label.Text = message;
            label.Editable = false;



        }
        else
        {
            _errorTooltip.Visible = false;
        }
    }


    //------------------------------------------------------------------------------------------------





    //-----------------------------------Canvas Manager ---------------------------------------------
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

    private void CloseResize()
    {
        _resizeInput.Visible = false;
    }


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






    //-----------------------------------File Manager ---------------------------------------------

    private void OnFileOpenSelected(string path)
    {
        if (!path.EndsWith(".pw"))
        {
            _consoleOutput.Text += "\n" + "Solo se admiten archivos terminados en .pw" + "\n" + userScript + "\n" + ">>>";
            return;
        }

        string fileContent = File.ReadAllText(path);
        _codeEditNode.Text = fileContent;
        filePath = path;
        CodeChange();
        _consoleOutput.ConsoleLog($"\n{infoConsole}");
        _consoleOutput.AppendPrompt();
    }

    private void OnFileSaveSelected(string path)
    {

        try
        {
            File.WriteAllText(path, _codeEditNode.Text);
            filePath = path;

        }
        catch (System.Exception error)
        {
            _consoleOutput.Text += "\n" + "Error al guardar el archivo" + error + "\n" + userScript + "\n" + ">>>";

        }
    }

    public void NewFile()
    {
        _codeEditNode.Text = "";
        filePath = null;
    }





    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("SaveCode"))
        {
            if (filePath == null)
            {
                _saveFileDialog.Popup();
            }
        }
        else
        {
            File.WriteAllText(filePath, _codeEditNode.Text);
        }

      
      
    }


    
    //------------------------------------------------------------------------------------------------
}