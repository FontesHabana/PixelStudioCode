using Godot;
using System;
using Godot.Collections;
using System.Text.RegularExpressions;

public partial class ConsoleHighlighter: SyntaxHighlighter{
    private Color bracketContentCOlor=new Color((float)0.5,(float)0.5,(float)1.0,1);
    TextEdit textEdit {get;set;}

    public ConsoleHighlighter(TextEdit text){
        textEdit=text;
    }
    public override Dictionary _GetLineSyntaxHighlighting(int line){
        string text= textEdit.GetLine(line);
        var highlighting=new Dictionary();
        var matches=Regex.Matches(text,@"\*\*\*(.*?)\*\*\*");
        var error = Regex.Matches(text, @"Error");
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

         foreach (Match match in error)
        {
            int contentStartIndex = match.Index;
            int contentEndIndex = match.Index + match.Length;

            if (match.Length > 0)
            {
                var style = new Dictionary{
                    {"color", new Color(255,0,0)}
                };


                highlighting[contentStartIndex] = style;
                if (contentEndIndex < text.Length)
                {
                    highlighting[contentEndIndex] = new Dictionary();
                }



            }

        }

        return highlighting;
    }
}