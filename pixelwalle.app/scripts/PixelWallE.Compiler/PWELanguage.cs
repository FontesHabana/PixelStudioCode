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
   public  List<PixelWallEException> Errors { get;  set;}
   public  Canvas Canvas { get;  set;}
   public Scope Scope;

    public Interpreter(Canvas canvas, string code)
  {
    Canvas = canvas;
    Errors = new List<PixelWallEException>();
    Run(code);
  }
    
  
   private void Run(string source){
    Errors=new List<PixelWallEException>();
    Lexer.Lexer lex = LexerProvider.Lexical;
    IEnumerable<Token> tokens= lex.GetTokens("test", source,Errors);
    TokenStream stream=new TokenStream(tokens);
    Parser parser=new Parser(stream,Errors);

    
    
    if (Errors.Count>0)
      {
        foreach (PixelWallEException error in Errors)
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
    foreach (PixelWallEException error in program.Errors)
        {
        GD.Print(error);
       // hadError=true;
        }
       // return;
   }
  printAst.printAstNode(program, 0);
 
  Scope=new Scope(program.Labels);
  Godot.GD.Print("Creado scope");
  SemanticChecker semanticChecker=new SemanticChecker(Scope, Errors);
Godot.GD.Print("Creado checkeco semantico");

      program.Accept(semanticChecker);

   
 


   
  if (semanticChecker.errors.Count>0)
  {
    foreach (var item in semanticChecker.errors)
    {
      GD.Print(item);
    }
  }
    if (Errors.Count==0)
    {
     RobotState robot=new RobotState();
 
  
    Executer executer=new Executer(Scope, Canvas, robot, Errors);
    program.Accept(executer);
    }
 

    /* for (int i = 0; i < Canvas.Size; i++)
        {
            for (int j = 0; j < Canvas.Size; j++)
            {
                GD.Print(Canvas.Matrix[i, j]);
            }
            GD.Print();
        }*/
  
   }



 

}