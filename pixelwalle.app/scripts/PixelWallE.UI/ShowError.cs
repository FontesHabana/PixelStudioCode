using Godot;
using System;
using System.Collections.Generic;

public partial class ShowError : ColorRect
{
	[Export] TextEdit _errorMessage;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CodeHighlighter mySyntaxHighlighter = (CodeHighlighter)_errorMessage.SyntaxHighlighter;
		mySyntaxHighlighter.AddKeywordColor("Error", new Godot.Color(1, 0, 0));
		
		
		_errorMessage.SyntaxHighlighter = mySyntaxHighlighter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	 }
