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

/// <summary>
/// The Interpreter class is responsible for interpreting and executing PixelWallE code.
/// It orchestrates the lexing, parsing, semantic checking, and execution phases.
/// </summary>
public  class Interpreter{
   /// <summary>
   /// A list to store any errors encountered during the interpretation process.
   /// </summary>
   public  List<PixelWallEException> Errors { get;  set;}
   


   public List<string> ConsoleMessage{ get; set; }





  /// <summary>
  /// The canvas on which the PixelWallE program will operate.
  /// </summary>
  public Canvas Canvas { get; set; }
   /// <summary>
   /// The scope that manages variables and labels during semantic analysis and execution.
   /// </summary>
   public Scope Scope{ get; set; }
  
  
   



   /// <summary>
  /// The parsed representation of the PixelWallE program.
  /// </summary>
  private ElementalProgram Program { get; set; }



  
  /// <summary>
  /// Initializes a new instance of the <see cref="Interpreter"/> class.
  /// </summary>
  /// <param name="canvas">The canvas to be used by the interpreter.</param>
  /// <param name="code">The PixelWallE source code to be interpreted.</param>
  public Interpreter(Canvas canvas, string code)
  {
    Canvas = canvas;
    Errors = new List<PixelWallEException>();
    ConsoleMessage = new List<string>();
    Interpetation(code);

  }


  /// <summary>
  /// Performs the complete interpretation process, from lexing to semantic checking.
  /// </summary>
  /// <param name="source">The PixelWallE source code to interpret.</param>
  public void Interpetation(string source)
  {
    Errors = new List<PixelWallEException>();

    //----------------Lexer------------------------------
    Lexer.Lexer lex = LexerProvider.Lexical;
    IEnumerable<Token> tokens = lex.GetTokens("test", source, Errors);
    TokenStream stream = new TokenStream(tokens);
   
    
   
   //-----------------Parser-----------------------------
    Parser parser = new Parser(stream, Errors);
    Program = parser.Parse();
    
   

    //---------------SemanticChecker-------------------------
    Scope = new Scope(Program.Labels);
    SemanticChecker semanticChecker = new SemanticChecker(Scope, Errors);
    Program.Accept(semanticChecker);


    //For debugger
    foreach (Token token in tokens)
    {
      GD.Print(token.ToString());
    }
    printAst.printAstNode(Program, 0);
  }
  
   /// <summary>
   /// Executes the interpreted PixelWallE program.
   /// </summary>
  public void Run()
  {
   
    if (Errors.Count == 0)
    {
      RobotState robot = new RobotState();


      Executer executer = new Executer(Scope, Canvas, robot, Errors, ConsoleMessage);
      Program.Accept(executer);
    }
  }



 

}