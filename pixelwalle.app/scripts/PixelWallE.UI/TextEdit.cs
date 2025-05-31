using Godot;
using System;
namespace Editor;
using System;
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
		Text += $"\n {text}";
	}



    private void ChangeTheme()
	{
		CodeHighlighter customHighlighter = new ConsoleHighlighter(this);
		CodeHighlighter custom2 = new CodeHighlighter();
		custom2.NumberColor = new Godot.Color(0.9f, 0.9f, 1f);
		custom2.AddKeywordColor("Error", new Color(1f, 0f, 0f));
		custom2.AddKeywordColor("Syntax", new Godot.Color(1f, 0f, 0f));
		custom2.AddKeywordColor("Lexical", new Godot.Color(1f, 0f, 0f));
		custom2.AddKeywordColor("Semantic", new Godot.Color(1f, 0f, 0f));
		custom2.AddKeywordColor("Runtime", new Godot.Color(1f, 0f, 0f));
		custom2.AddColorRegion("***", "***", new Godot.Color(0.5f, 0.5f, 1f));
		custom2.FunctionColor = new Godot.Color(199 / 255f, 148 / 255f, 157 / 255f);
		custom2.SymbolColor = new Godot.Color(1f, 1f, 1f);

		SyntaxHighlighter = customHighlighter;
		SyntaxHighlighter = custom2;
	}
}