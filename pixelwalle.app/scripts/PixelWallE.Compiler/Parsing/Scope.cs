using PixelWallE.Language.Parsing.Expressions;
using System.Collections.Generic;
using System.Drawing;


namespace PixelWallE.Language.Parsing;

public class Scope
{   //Permite extender el lenguaje a contextos globales y bloques de codigo
    public readonly Dictionary<string, (object, ExpressionType)> variables = new();
    public readonly Dictionary<string, Label> labels = new();

    public readonly Dictionary<string, Godot.Color> colors = new();
    //
    private readonly Scope? parent;
    //Se pueden agregar las funciones aquí;
    public Scope(List<Label> labels, Scope? parent = null)
    {
        try
        {
              this.parent = parent;
        foreach (var item in labels)
        {
            this.labels.Add(item.LabelReference, item);
        }
        }
        catch (System.Exception error)
        {
             Godot.GD.Print(error);
           
        }
      
        Godot.GD.Print("Labels añadidos con exito");
        colors = new()
        {
             {"Transparent", new  Godot.Color(255, 255, 255, 0)},
            {"Red", new  Godot.Color(255, 0, 0)},
            {"Blue", new  Godot.Color(0, 0, 255)},
            {"Green", new  Godot.Color(0, 255, 0)},
            {"Yellow", new  Godot.Color(255, 255, 0)},
            {"Orange", new  Godot.Color(255, 165, 0)},
            {"Purple", new  Godot.Color(160, 32, 240)},
            {"Black", new  Godot.Color(0, 0, 0)},
            {"White", new  Godot.Color(255, 255, 255)},
            {"Pink", new  Godot.Color(255, 80, 220)},
        };
        Godot.GD.Print("Scope creado con exito");
    }


    public bool IsDeclared<T>(string name, Dictionary<string, T> dic)
    {
        if (dic.ContainsKey(name))
            return true;
        else if (parent != null)
        {
            return parent.IsDeclared(name, dic);
        }
        return false;
    }
    public bool DeclareVariable(string name, object value, ExpressionType type)
    {
        if (variables.ContainsKey(name))
        {
            return false;
        }
        variables[name] = (value, type);
        return true;
    }

    public bool AssignVariable(string name, object value, ExpressionType type)
    {
        if (variables.ContainsKey(name))
        {
            variables[name] = (value, type);
            return true;
        }
        else if (parent != null)
        {
            return parent.AssignVariable(name, value, type);
        }

        return false;
    }

    public object? GetVariable(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name].Item1;
        }
        return parent?.GetVariable(name);
    }

    public ExpressionType GetVariableType(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name].Item2;
        }
        if (parent == null)
        {
            return ExpressionType.Anytype;
        }
        return parent.GetVariableType(name);
    }




    public Label? GetLabel(string name)
    {
        if (labels.ContainsKey(name))
        {
            return labels[name];
        }
        return parent?.GetLabel(name);
    }


public Godot.Color? GetColor(string name)
    {
        if (colors.ContainsKey(name))
        {
            return colors[name];
        }
        return parent?.GetColor(name);
    }

    //Aqui hay DRY revisa



}