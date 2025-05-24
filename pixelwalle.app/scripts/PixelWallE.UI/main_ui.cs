using Godot;
using System;
using System.IO;
using System.Threading.Tasks;
using PixelWallE.Language;
using PixelWallE.Core;
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
    private Button _processButton;
    private TextEdit _textEditOutput;
    private static Canvas canvas = new Canvas(25);
    public static Interpreter interpreter = new Interpreter(canvas, ""); // Inicializa el intérprete con un tamaño de 25 (ajusta según sea necesario)



    // Método _Ready se llama cuando el nodo y sus hijos han entrado en el árbol de escena
    public override void _Ready()
    {
        GD.Print("--> Método _Ready() iniciado.");


        
        // *** ESTAS SON LAS LÍNEAS QUE NECESITAN CAMBIAR ***
        // Debes poner las rutas CORRECTAS SEGÚN LA JERARQUÍA DE TU ESCENA

        _processButton = GetNode<Button>("./Button");
        _textEditOutput = GetNode<TextEdit>("./TextEdit");
        // ***************************************************

        // El resto del código de _Ready() y los otros métodos sigue siendo válido
        // (las verificaciones de null, la conexión de señal, etc.)



        _processButton.Pressed += OnButtonPressed;
        

        if (_textEditOutput != null)
        {
            // _textEditOutput.Editable = false;
        }
        var myHighlighter = new CodeHighlighter(); // Sí, se llama GDScriptHighlighter incluso en C# para resaltar sintaxis GDScript
        myHighlighter.AddKeywordColor("true", Colors.Red);
        myHighlighter.AddKeywordColor("false", Colors.Red);


        

        myHighlighter.NumberColor = Colors.Blue;
        myHighlighter.AddKeywordColor("\"", Colors.Yellow);
        myHighlighter.SymbolColor=Colors.Aqua;
        myHighlighter.FunctionColor=Colors.Green;
        myHighlighter.MemberVariableColor = Colors.Orange;
        myHighlighter.AddKeywordColor("GoTo", Colors.Coral);
     


        _codeEditNode.SyntaxHighlighter = myHighlighter;

        GD.Print("--> Método _Ready() finalizado.");
    }

    // Este método recibirá el texto del CodeEdit
    private void ProcessCode(string codeString)
    {
        interpreter = new Interpreter(canvas, codeString);
        GD.Print(interpreter.Canvas.Matrix[0, 0]);
        GD.Print("Método 'ProcessCode' llamado con el siguiente texto:");
        GD.Print(codeString); // Imprime en la consola de salida de Godot

        string userScript = "***D:/.../Archivo  Fecha 🤖***";
        // Aquí puedes añadir lógica para 'ejecutar' o 'analizar' el código
        // ... (Por ahora, solo imprimimos y actualizamos el TextEdit)
        // Mostrar el texto recibido en el TextEdit
        foreach (CompilingError error in interpreter.Errors)
        {
            _textEditOutput.Text +="\n"+ error.ToString() ;
        }
        if (interpreter.Errors.Count==0)
        {
             _textEditOutput.Text +="\n Compilado correctamente" ;
        }
       

        _textEditOutput.Text += "\n"+  userScript + "\n" + ">>>";


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

    }

    // Limpieza cuando el nodo va a ser eliminado (si conectaste señales en _Ready)
    public override void _ExitTree()
    {
        if (_processButton != null)
        {
            _processButton.Pressed -= OnButtonPressed; // Desconecta la señal para evitar errores
        }
    }

    private void PressedSave()
    {
        _saveFileDialog.Popup();
    }
    private void PressedLoad()
    {
        _loadFileDialog.Popup();
    }

    private async void ResizePressed()
    {
        ResizedCanvas(_resizeInput.Text);   
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
    private void PressedVista()
    {
       
    }

    private void OnFileSelected(string path)
    {
        if (!path.EndsWith(".pw"))
        {
            ProcessCode("Solo se admiten archivos terminados en .pw");
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
        {
            ProcessCode("Error al guardar el archivo" + error);
        }
    }

    private void ResizedCanvas(string text)
    {
        int number;
        if (int.TryParse(text, out number))
        {
            if (number <= 256 && number>0)
            {
                canvas = new Canvas(number);
                interpreter.Canvas = canvas;
                
            }
        }
        _canvas._Ready();   
        _canvas.QueueRedraw();
        _resizeInput.Text = "";
        _resizeInput.PlaceholderText = canvas.Size.ToString();

    }
    //opcion 1 presionando enter en el teclado
   

    public void SaveTextureRectAsImage(TextureRect textureRect, string filePath)
    {
        Texture2D texture = textureRect.Texture;
        Image image = texture.GetImage();
        Error error = image.SavePng(filePath);
        if (error == Error.Ok)
        {
            GD.Print("Salvada");
        }
        else
        {
            GD.Print("no salvada");
        }
    }


}