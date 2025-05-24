using System.Runtime.InteropServices;
using PixelWallE.Language.Lexer;
using PixelWallE.Language;
using PixelWallE.Language.Tokens;
using PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Core;
using Godot;
using System.Collections.Generic;
using GodotPlugins.Game;

namespace PixelWallE.Language;

public  class Interpreter{
   public  List<CompilingError> Errors { get;  set;}
   public  Canvas Canvas { get;  set;} 

    public Interpreter(Canvas canvas, string code){
      Canvas = canvas;
      Errors=new List<CompilingError>();
      Run(code);
    }
    
  
   private void Run(string source){
    Errors=new List<CompilingError>();
    Lexer.Lexer lex = LexerProvider.Lexical;
    IEnumerable<Token> tokens= lex.GetTokens("test", source,Errors);
    TokenStream stream=new TokenStream(tokens);
    Parser parser=new Parser(stream,Errors);

    
    
    if (Errors.Count>0)
      {
        foreach (CompilingError error in Errors)
        {
        GD.Print(error);
        //hadError=true;
        }
        //return;
      }
    foreach (Token token in tokens)
      {
        GD.Print(token.toString());
      }
   

   ElementalProgram program=parser.Parse();
   if (program.Errors.Count>0)
   { 
    foreach (CompilingError error in program.Errors)
        {
        GD.Print(error);
       // hadError=true;
        }
       // return;
   }
  printAst.printAstNode(program, 0);
   
  Scope scope=new Scope(program.Labels);
  SemanticChecker semanticChecker=new SemanticChecker(scope, Errors);
  program.Accept(semanticChecker);



  if (semanticChecker.errors.Count>0)
  {
    foreach (var item in semanticChecker.errors)
    {
      GD.Print(item);
    }
  }
    
    RobotState robot=new RobotState();
    Executer executer=new Executer(scope, Canvas, robot, Errors);
    program.Accept(executer);

    /* for (int i = 0; i < Canvas.Size; i++)
        {
            for (int j = 0; j < Canvas.Size; j++)
            {
                GD.Print(Canvas.Matrix[i, j]);
            }
            GD.Print();
        }*/
  
   }


    public static void error(CodeLocation location, string message) {
    report(location, "", message);
    }
    private static void report(CodeLocation location, string where,
    string message) {
    GD.Print(
    "[line " + location.Line + "][column" +location.Column+ "] Error" + where + ": " + message);
   // hadError = true;
    }

 

}