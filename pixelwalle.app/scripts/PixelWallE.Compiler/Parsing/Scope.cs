using PixelWallE.Language.Parsing.Expressions;
using System.Collections.Generic;
using System.Drawing;


namespace PixelWallE.Language.Parsing;

/// <summary>
/// Represents a scope in the language, managing variables, labels, and colors.
/// Scopes can be nested to handle variable shadowing and hierarchical lookups.
/// </summary>
public class Scope
{   //Permite extender el lenguaje a contextos globales y bloques de codigo
    /// <summary>
    /// Dictionary storing variables with their values and types.
    /// </summary>
    public readonly Dictionary<string, (object, ExpressionType)> variables = new();
    /// <summary>
    /// Dictionary storing labels for goto statements.
    /// </summary>
    public readonly Dictionary<string, Label> labels = new();

    /// <summary>
    /// Dictionary storing predefined colors.
    /// </summary>
    public readonly Dictionary<string, Godot.Color> colors = new()
    {
          {"Transparent", new  Godot.Color(1f, 1f, 1f, 0)},
            {"Red", new  Godot.Color(1f, 0, 0)},
            {"Blue", new  Godot.Color(0, 0, 1f)},
            {"Green", new  Godot.Color(0, 1f, 0f)},
            {"Yellow", new  Godot.Color(1f, 1f, 0f)},
            {"Orange", new  Godot.Color(1f, 165/255f, 0/255f)},
            {"Purple", new  Godot.Color(160/255f, 32/255f, 240/255f)},
            {"Black", new  Godot.Color(0f, 0f, 0f)},
            {"White", new  Godot.Color(1f, 1f, 1f)},
            {"Pink", new  Godot.Color(1f, 80/255f, 220/255f)},

    };
    //
    /// <summary>
    /// The parent scope, used for resolving variables and labels in outer scopes.
    /// </summary>
    private readonly Scope? parent;
    //Se pueden agregar las funciones aquí;
    /// <summary>
    /// Initializes a new instance of the <see cref="Scope"/> class.
    /// </summary>
    /// <param name="initialLabels">Initial labels to add to the scope.</param>
    /// <param name="parent">The parent scope.</param>
    public Scope(List<Label> initialLabels, Scope? parent = null)
    {


        this.parent = parent;
        if (initialLabels != null)
        {
            foreach (Label item in initialLabels)
            {
                labels.Add(item.LabelReference, item);
            }
        }

    }

    /// <summary>
    /// Checks if a name is declared in the scope or any of its parent scopes.
    /// </summary>
    /// <typeparam name="T">The type of the dictionary value.</typeparam>
    /// <param name="name">The name to check.</param>
    /// <param name="dic">The dictionary to check in.</param>
    /// <returns>True if the name is declared, false otherwise.</returns>
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




    #region Variable
    /// <summary>
    /// Declares a variable in the current scope.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value of the variable.</param>
    /// <param name="type">The type of the variable.</param>
    /// <returns>True if the variable was declared, false otherwise (if it already exists in the current scope).</returns>
    public bool DeclareVariable(string name, object value, ExpressionType type)
    {
        if (variables.ContainsKey(name))
        {
            return false;
        }
        variables[name] = (value, type);
        return true;
    }

    /// <summary>
    /// Assigns a value to a variable in the current scope or a parent scope.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <param name="value">The value to assign.</param>
    /// <param name="type">The type of the variable.</param>
    /// <returns>True if the variable was assigned, false otherwise (if it doesn't exist in the scope hierarchy).</returns>
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

    /// <summary>
    /// Gets the value of a variable from the current scope or a parent scope.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The value of the variable, or null if not found.</returns>
    public object? GetVariable(string name)
    {
        if (variables.ContainsKey(name))
        {
            return variables[name].Item1;
        }
        return parent?.GetVariable(name);
    }

    /// <summary>
    /// Gets the type of a variable from the current scope or a parent scope.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The type of the variable. Returns ExpressionType.Anytype if not found.</returns>
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
    #endregion
    #region Label
    /// <summary>
    /// Gets a label from the current scope or a parent scope.
    /// </summary>
    /// <param name="name">The name of the label.</param>
    /// <returns>The label if found, otherwise null.</returns>
    public Label? GetLabel(string name)
    {
        if (labels.ContainsKey(name))
        {
            return labels[name];
        }
        return parent?.GetLabel(name);
    }
    #endregion
    #region Color
    /// <summary>
    /// Gets a color from the current scope or a parent scope.
    /// </summary>
    /// <param name="name">The name of the color.</param>
    /// <returns>The color if found, otherwise null.</returns>
    public Godot.Color? GetColor(string name)
    {
        if (colors.ContainsKey(name))
        {
            return colors[name];
        }
        return parent?.GetColor(name);
    }
    #endregion
}