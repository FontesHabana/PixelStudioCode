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
		mySyntaxHighlighter.AddKeywordColor("Error", new Godot.Color(255, 0, 0));
		mySyntaxHighlighter.AddKeywordColor("Syntax", new Godot.Color(255, 0, 0));
		mySyntaxHighlighter.AddKeywordColor("Lexical", new Godot.Color(255, 0, 0));
		mySyntaxHighlighter.AddKeywordColor("Semantic", new Godot.Color(255, 0, 0));
		mySyntaxHighlighter.AddKeywordColor("line", new Godot.Color(0, 207, 255,1));
		mySyntaxHighlighter.AddKeywordColor("column", new Godot.Color(0, 207, 255,1));
		_errorMessage.SyntaxHighlighter = mySyntaxHighlighter;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}



	 }
