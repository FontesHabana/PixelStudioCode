using Godot;
using System;
using Godot.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace Editor;
public partial class ConsoleHighlighter: SyntaxHighlighter{
    private Godot.Color bracketContentCOlor=new Godot.Color(149,0,20);
    TextEdit textEdit {get;set;}

    public ConsoleHighlighter(TextEdit text){
        textEdit=text;
    }
    public override Dictionary _GetLineSyntaxHighlighting(int line){
        string text= textEdit.GetLine(line);
        var highlighting=new Dictionary();
        var matches=Regex.Matches(text,@"\*\*\*(.*?)\*\*\*");
        //var error = Regex.Matches(text, @"Error");
        List<string> errors = new List<string> { "Error", "Lexical", "Syntax", "Semantic" };
        foreach (Match match in matches)
        {
            int contentStartIndex = match.Index;
            int contentEndIndex = match.Index + match.Length;

            if (match.Length > 0)
            {
                var style = new Dictionary{
                    {"color",bracketContentCOlor}
                };


                highlighting[contentStartIndex] = style;
                if (contentEndIndex < text.Length)
                {
                    highlighting[contentEndIndex] = new Dictionary();
                }



            }

        }


        foreach (string item in errors)
        {
            var error= Regex.Matches(text, $@"{item}");
              foreach (Match match in error)
        {
            int contentStartIndex = match.Index;
            int contentEndIndex = match.Index + match.Length;

            if (match.Length > 0)
            {
                var style = new Dictionary{
                    {"color", new Godot.Color(255, 0, 0)}
                };


                highlighting[contentStartIndex] = style;
                if (contentEndIndex < text.Length)
                {
                    highlighting[contentEndIndex] = new Dictionary();
                }



            }

        }
        }
      

        return highlighting;
    }
}