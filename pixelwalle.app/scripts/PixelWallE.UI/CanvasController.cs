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
    {   
        Interpreter interpreter = main_ui.interpreter;
        
        int size = interpreter.Canvas.Size;
        float space = Size.X / size;
        


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
               
                Rect2 rect = new Rect2(j * space, i * space, space, space);
                DrawRect(rect, color);

            }
        }
    }
   
    public Godot.Color CheckColor(PixelColor color)
    {
        return new Godot.Color(color.Red / 255f, color.Green / 255f, color.Blue / 255f, color.Alpha / 255f);
    }



}
