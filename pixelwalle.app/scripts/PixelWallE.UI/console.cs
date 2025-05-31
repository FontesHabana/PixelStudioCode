using Godot;
using System;
namespace Editor;
using System;
using Godot.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public partial class ConsoleHighlighter: CodeHighlighter{
    
    
    private Godot.Color bracketContentCOlor = new Godot.Color(0.5f, 0.5f, 0.9f);
    TextEdit textEdit {get;set;}

    public ConsoleHighlighter(TextEdit text)
    {
        textEdit = text;
        
    }
    public override Dictionary _GetLineSyntaxHighlighting(int line){
        string text= textEdit.GetLine(line);
        var highlighting=new Dictionary();
        var matches=Regex.Matches(text,@"\*\*\*(.*?)\*\*\*");
        
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


        

        return highlighting;
    }
}