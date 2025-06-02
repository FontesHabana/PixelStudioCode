using Godot;
using System;
namespace Editor;
using System;
using MyConsole;
using Godot.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public partial class TextEdit : Godot.TextEdit
{
	private int protectedOffset = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AppendPrompt();
		ChangeTheme();

	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if (GetCaretLine() == GetLineCount() - 1 && GetCaretColumn() <= protectedOffset)
			{
				if (keyEvent.Keycode == Key.Backspace ||
				keyEvent.Keycode == Key.Delete ||
				keyEvent.Keycode == Key.Left ||
				keyEvent.Keycode == Key.Home)
				{
					AcceptEvent();
					return;
				}
			}

			GD.Print("Voy a ejecutar enter");
			if (keyEvent.Keycode == Key.Enter)
			{	AcceptEvent();
				GD.Print("lo hice enter");

				MyConsole.Console.HandleInput(ExtractFromUntilChar(this.Text, '>'), (main_ui)GetParent());

				return;
			}

			}


	}
		

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (GetCaretLine() != GetLineCount() - 1 || GetCaretColumn() < protectedOffset)
		{
			SetCaretLine(GetLineCount() - 1);
			SetCaretColumn(protectedOffset);
		}
	}

	public void AppendPrompt(string prompt = "\n>>>")
	{
		Text += prompt;
		protectedOffset = GetLine(GetLineCount() - 1).Length;
		SetCaretLine(GetLineCount() - 1);
		SetCaretColumn(protectedOffset);
		GrabFocus();
	}


	public void ConsoleLog(string text)
	{
		Text += text;
	}


	  private string ExtractFromUntilChar(string input, char delimiter)
    {
        int index = input.LastIndexOf(delimiter);

        if (index == -1)
        {
            return input;
        }

        return input.Substring(index + 1);
    }


	private void ChangeTheme()
	{
		CodeHighlighter custom = new CodeHighlighter();
		custom.NumberColor = new Godot.Color(0.9f, 0.9f, 1f);
		custom.AddKeywordColor("Error", new Color(1f, 0f, 0f));
		custom.AddKeywordColor("Syntax", new Godot.Color(1f, 0f, 0f));
		custom.AddKeywordColor("Lexical", new Godot.Color(1f, 0f, 0f));
		custom.AddKeywordColor("Semantic", new Godot.Color(1f, 0f, 0f));
		custom.AddKeywordColor("Runtime", new Godot.Color(1f, 0f, 0f));

		custom.AddKeywordColor("run",new Godot.Color(0.392f, 0.714f, 1.0f));  
		custom.AddKeywordColor("Run",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("clear",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Clear",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("console",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Console",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Canvas",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("canvas",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("resize",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Resize",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("file",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("File",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("undo",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Undo",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("redo",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Redo",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("new",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("New",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("show",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Show",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("commands",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Commands",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("Help",new Godot.Color(0.392f, 0.714f, 1.0f)); 
		custom.AddKeywordColor("help",new Godot.Color(0.392f, 0.714f, 1.0f)); 



		custom.AddColorRegion("***", "***", new Godot.Color(0.5f, 0.5f, 1f));
		custom.FunctionColor = new Godot.Color(199 / 255f, 148 / 255f, 157 / 255f);
		custom.SymbolColor = new Godot.Color(1f, 1f, 1f);
		custom.MemberVariableColor = new Godot.Color(199 / 255f, 148 / 255f, 157 / 255f);

		SyntaxHighlighter = custom;
	}
}