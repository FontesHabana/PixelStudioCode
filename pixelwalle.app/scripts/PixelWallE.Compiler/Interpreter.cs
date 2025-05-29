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
  
   private ElementalProgram Program{ get; set; }

  public Interpreter(Canvas canvas, string code)
  {
    Canvas = canvas;
    Errors = new List<PixelWallEException>();
    Interpetation(code);
    
  }




  public void Interpetation(string source)
  {
    Errors = new List<PixelWallEException>();
    Lexer.Lexer lex = LexerProvider.Lexical;
    IEnumerable<Token> tokens = lex.GetTokens("test", source, Errors);
    TokenStream stream = new TokenStream(tokens);
    Parser parser = new Parser(stream, Errors);



    if (Errors.Count > 0)
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


    Program = parser.Parse();
    if (Program.Errors.Count > 0)
    {
      foreach (PixelWallEException error in Program.Errors)
      {
        GD.Print(error);
        // hadError=true;
      }
      // return;
    }
    printAst.printAstNode(Program, 0);

    Scope = new Scope(Program.Labels);
    Godot.GD.Print("Creado scope");
    SemanticChecker semanticChecker = new SemanticChecker(Scope, Errors);
    Godot.GD.Print("Creado checkeco semantico");

    Program.Accept(semanticChecker);






    if (semanticChecker.errors.Count > 0)
    {
      foreach (var item in semanticChecker.errors)
      {
        GD.Print(item);
      }
    }
  }
  
   public void Run()
  {
   
    if (Errors.Count == 0)
    {
      RobotState robot = new RobotState();


      Executer executer = new Executer(Scope, Canvas, robot, Errors);
      Program.Accept(executer);
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