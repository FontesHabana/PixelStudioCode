using Godot;
using System;
namespace Editor;
public partial class TextEdit : Godot.TextEdit
{
	private int protectedOffset=0 ;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AppendPrompt();
		SyntaxHighlighter customHighlighter = new ConsoleHighlighter(this);
		this.SyntaxHighlighter = customHighlighter;
	
		
		
		
	}

    public override void _GuiInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed)
		{
			if(GetCaretLine()==GetLineCount()-1&& GetCaretColumn()<=protectedOffset){
				if (keyEvent.Keycode==Key.Backspace||
				keyEvent.Keycode==Key.Delete||
				keyEvent.Keycode==Key.Left||
				keyEvent.Keycode==Key.Home)
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
		if (GetCaretLine()!=GetLineCount()-1|| GetCaretColumn() <protectedOffset)
		{
			SetCaretLine(GetLineCount()-1);
			SetCaretColumn(protectedOffset);
		}
	}

	public void AppendPrompt(string prompt="\n>>>"){
		Text+=prompt;
		protectedOffset=GetLine(GetLineCount()-1).Length;
		SetCaretLine(GetLineCount()-1);
		SetCaretColumn(protectedOffset);
		GrabFocus();
	}


}
