using Godot;
using System;
namespace Editor;

public partial class CodeEdit : Godot.CodeEdit
{
	
    public override void _Ready()
    {
    
        ChangeTheme();

    }
    
    private void ChangeTheme()
	{
		 var myHighlighter = new CodeHighlighter(); // Sí, se llama GDScriptHighlighter incluso en C# para resaltar sintaxis GDScript
         myHighlighter.AddKeywordColor("true", new Godot.Color(0.643f, 0.545f, 1.0f));     // Violeta eléctrico
         myHighlighter.AddKeywordColor("false", new Godot.Color(0.643f, 0.545f, 1.0f));    // Igual que true
         myHighlighter.NumberColor = new Godot.Color(1.0f, 0.592f, 0.71f);                 // Rosa fuerte
     

        myHighlighter.SymbolColor = new Godot.Color(1.0f, 0.914f, 0.49f);                 // Amarillo claro
        myHighlighter.FunctionColor = new Godot.Color(0.392f, 0.714f, 1.0f);              // Azul neón
        myHighlighter.MemberVariableColor = new Godot.Color(1.0f, 0.592f, 0.71f);         // Igual que números

        myHighlighter.AddKeywordColor("GoTo", new Godot.Color(0.643f, 0.545f, 1.0f));

  
        myHighlighter.AddColorRegion("#","", new Godot.Color(0.5f, 0.5f, 0.5f));
        myHighlighter.AddColorRegion("\"", "\"", new Godot.Color(1.0f, 0.416f, 0.416f));

       

      

        

		this.SyntaxHighlighter = myHighlighter;
	}
}


