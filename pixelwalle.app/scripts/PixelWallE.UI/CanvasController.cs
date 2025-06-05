using Godot;
using PixelWallE.Core;
using PixelWallE.Language;
using PixelWallE.Language.Parsing;
using System;
namespace Editor;
public partial class CanvasController : TextureRect
{
    Godot.Color GridColor = new Godot.Color(0, 0, 0, 0.1f);
    [Export] float LineWidth = 1.0f;



    public override void _Draw()
    {   //Tomar el tamaño del canvas de una casilla
        Interpreter interpreter = main_ui.interpreter;
        
        int size = interpreter.Canvas.Size;
        float space = Size.X / size;
        GD.Print(Size.X);
        GD.Print(size);
        GD.Print(space);


        DrawColor(size, space, interpreter);
        if (interpreter.Canvas.Size<100)
        {
             for (int i = 1; i < size; i++)
        {
            float c = i * space;
            DrawLine(new Vector2(0, c), new Vector2(Size.X, c), GridColor, LineWidth);
            DrawLine(new Vector2(c, 0), new Vector2(c, Size.Y), GridColor, LineWidth);
        }
        }
       
    }
    private void DrawColor(int size, float space, Interpreter interpreter)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Godot.Color color = CheckColor(interpreter.Canvas.Matrix[i, j]);
                //GD.Print(interpreter.Canvas.Matrix[i, j]);
                //  GD.Print("dibuje pixel"+color.ToString());
                Rect2 rect = new Rect2(j * space, i * space, space, space);
                DrawRect(rect, color);
                //DrawRect(new Rect2(i, j, space, space), new Godot.Color(255, 0, 255));

            }
        }
    }
    /*  private void UpdateBrushPosition(float space)
      {
          (int x, int y) = Scope.Position;
          Vector2 currentPosition = new Vector2(x, y);

          float X = currentPosition.X * space + 20;
          float Y = currentPosition.Y * space - 20;

          Brush.Position = new Vector2(X, Y);
      }*/
    public Godot.Color CheckColor(PixelColor color)
    {
        return new Godot.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f, color.Alpha / 255f);
    }



}
