using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using PixelWallE.Language;
using PixelWallE.Core;
using System.Collections.Generic;
using System.Linq;
public partial class main_ui : Control // partial es importante si adjuntas el script desde el editor
{
    // Declara variables miembro para guardar las referencias a los nodos
    [Export] CodeEdit _codeEditNode;
    [Export] FileDialog _saveFileDialog;
    [Export] FileDialog _loadFileDialog;
    [Export] MenuButton _controlArchives;
    [Export] CanvasController _canvas;
    [Export] Button _resizeButton;
    [Export] LineEdit _resizeInput;
    [Export] Button _cleanCanvas;
    [Export] Button _cleanConsole;
    [Export] Button _processButton;
    [Export] Button _closeButton;
    [Export] Button _goBackButton;
    [Export] Button _goNextButton;
    [Export] TextEdit _textEditOutput;
    private static Canvas canvas = new Canvas(25);
    private static Stack<Canvas> stackGoBack;
    private static Stack<Canvas> stackGoNext;
    private static Stack<int> eso=new Stack<int>();
    public static Interpreter interpreter = new Interpreter(canvas, ""); // Inicializa el intérprete con un tamaño de 25 (ajusta según sea necesario)
    string userScript = "***D:/.../Archivo  Fecha 🤖***";


    // Método _Ready se llama cuando el nodo y sus hijos han entrado en el árbol de escena
    




    public override void _Ready()
    {
        GD.Print("--> Método _Ready() iniciado.");



        // *** ESTAS SON LAS LÍNEAS QUE NECESITAN CAMBIAR ***
        // Debes poner las rutas CORRECTAS SEGÚN LA JERARQUÍA DE TU ESCENA

        // ***************************************************

        // El resto del código de _Ready() y los otros métodos sigue siendo válido
        // (las verificaciones de null, la conexión de señal, etc.)




        if (_textEditOutput != null)
        {
            // _textEditOutput.Editable = false;
        }
        var myHighlighter = new CodeHighlighter(); // Sí, se llama GDScriptHighlighter incluso en C# para resaltar sintaxis GDScript
        myHighlighter.AddKeywordColor("true", Colors.Red);
        myHighlighter.AddKeywordColor("false", Colors.Red);




        myHighlighter.NumberColor = Colors.Blue;
        myHighlighter.AddKeywordColor("\"", Colors.Yellow);
        myHighlighter.SymbolColor = Colors.Aqua;
        myHighlighter.FunctionColor = Colors.Green;
        myHighlighter.MemberVariableColor = Colors.Orange;
        myHighlighter.AddKeywordColor("GoTo", Colors.Coral);



        _codeEditNode.SyntaxHighlighter = myHighlighter;



        stackGoBack = new Stack<Canvas>();
        stackGoNext = new Stack<Canvas>();

        GD.Print("--> Método _Ready() finalizado.");
    }



    public override void _PhysicsProcess(double delta)
    {
        if (true)
        {
            _canvas.QueueRedraw();
        }
    }
    // Este método recibirá el texto del CodeEdit
    //hacer que este metodo genere la matriz copia completa y asi no tener que crearla de forma manual en el  proceso
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


    private void CodeChange()[
    ]
    private void ProcessCode(string codeString)
    {

        Canvas current = new Canvas(canvas.Size);
        CopyMatrix(canvas, current);
        stackGoBack.Push(current);
        interpreter = new Interpreter(canvas, codeString);






        _textEditOutput.Text += "\n" + eso.Count;
        GD.Print(interpreter.Canvas.Matrix[0, 0]);
        GD.Print("Método 'ProcessCode' llamado con el siguiente texto:");
        GD.Print(codeString); // Imprime en la consola de salida de Godot

        // string userScript = "***D:/.../Archivo  Fecha 🤖***";
        // Aquí puedes añadir lógica para 'ejecutar' o 'analizar' el código
        // ... (Por ahora, solo imprimimos y actualizamos el TextEdit)
        // Mostrar el texto recibido en el TextEdit
        foreach (PixelWallEException error in interpreter.Errors)
        {
            _textEditOutput.Text += "\n" + error.Message;
        }
        if (interpreter.Errors.Count == 0)
        {
            _textEditOutput.Text += "\n Compilado correctamente";
        }


        _textEditOutput.Text += "\n" + userScript + "\n" + ">>>";


        _textEditOutput.ScrollVertical = _textEditOutput.GetLineCount();
        _canvas.QueueRedraw();

        // Si quisieras añadir líneas nuevas en lugar de reemplazar, podrías hacer:
        // _textEditOutput.Text += "Texto recibido: " + codeString + "\n";


        // await ToSignal(GetTree(), "process_frame");

    }

    
    // Función que se llamará cuando se presione el botón (conectada en _Ready o en el editor)
    private void OnButtonPressed()
    {
        string codeFromEdit = _codeEditNode.Text; // Obtiene el texto del CodeEdit
        ProcessCode(codeFromEdit); // Llama al método de procesamiento con el texto
        _goBackButton.Visible = true;
    }

    // Limpieza cuando el nodo va a ser eliminado (si conectaste señales en _Ready)
    

    private void PressedSave()
    {
        _saveFileDialog.Popup();
    }
    private void PressedLoad()
    {
        _loadFileDialog.Popup();
    }
    private void CleanCanvas()
    {
        Canvas current = new Canvas(canvas.Size);
        CopyMatrix(interpreter.Canvas, current);
        stackGoBack.Push(current);
        canvas = new Canvas(canvas.Size);
        interpreter.Canvas = canvas;
        _canvas._Ready();
        _canvas.QueueRedraw();
       
    }
    private void CleanConsole()
    {
        _textEditOutput.Text = userScript + "\n" + ">>>";
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
   
    private void ClosePressed()
    {
        GetTree().Quit();
    }
    private void PressedArchiveControl()
    {
        _controlArchives.GetPopup().IdPressed += OnMenuItemPressed;
    }
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

    private void GoBack()
    {

        if (stackGoBack.Count > 0)
        {
            _textEditOutput.Text += "GoBack entre ";
          
            if (stackGoBack.Count > 1)
            {     Canvas current = new Canvas(canvas.Size);
                  CopyMatrix(interpreter.Canvas, current);
                  stackGoNext.Push(current);
                  interpreter.Canvas = stackGoBack.Pop();
            }
            else
            {
                 interpreter.Canvas = stackGoBack.Peek();
            }
           
            _canvas.QueueRedraw();
        }
        _goNextButton.Visible = true;
        

    }

    private void GoNext()
    {_textEditOutput.Text += "GoNext ";
       if (stackGoNext.Count > 0)
        {   Canvas current = new Canvas(canvas.Size);
        CopyMatrix(interpreter.Canvas, current);
            stackGoBack.Push(current);
            interpreter.Canvas = stackGoNext.Pop();
            _canvas.QueueRedraw();
        }
       
    }
    
    private void PressedVista()
    {

    }

    private void OnFileSelected(string path)
    {
        if (!path.EndsWith(".pw"))
        {   
             _textEditOutput.Text += "\n"+ "Solo se admiten archivos terminados en .pw"+"\n"+ userScript + "\n" + ">>>";
            return;
        }

        string fileContent = File.ReadAllText(path);
        _codeEditNode.Text = fileContent;
    }

    private void OnFileSaveSelected(string path)
    {

        try
        {   
            File.WriteAllText(path, _codeEditNode.Text);

        }
        catch (System.Exception error)
        {   _textEditOutput.Text += "\n"+ "Error al guardar el archivo" + error+"\n"+ userScript + "\n" + ">>>";
        
        }
    }

    private void ResizedCanvas(string text)
    {
        int number;
        if (int.TryParse(text, out number))
        {
            if (number <= 1024 && number > 0)
            {
                canvas = new Canvas(number);
                interpreter.Canvas = canvas;

            }
        }
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
        _resizeInput.Visible = false;

    }
    //opcion 1 presionando enter en el teclado
   

    


}