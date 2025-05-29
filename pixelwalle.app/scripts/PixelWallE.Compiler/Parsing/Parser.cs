using PixelWallE.Language.Parsing.Expressions.Literals;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using PixelWallE.Language.Expressions;
using System.Text.RegularExpressions;
using System.Runtime.ConstrainedExecution;
using PixelWallE.Language.Commands;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design;


namespace PixelWallE.Language.Parsing;

public class Parser{

    public TokenStream Stream  {get; private set;}
    private List<PixelWallEException> Errors{ get; set; }


    public Parser(TokenStream stream, List<PixelWallEException> errors)
    {
        Stream = stream;
        Errors = errors;
    }

 /*
    public IExecutable ParseInstruction(List<PixelWallEException> errors){
            Token tokenHead=Stream.Peek();

            if (tokenHead.Type==TokenType.IDENTIFIER)
            {
                //Parse asignación
            }
            //Para parsear las instrucciones crear un diccionario con todos los tipos de instrucción
            if (true)
            {
                
            }


    }
    
*/
#region Expression parsing
//separar esto en diferentes tipos de expresiones, por ejemplo, una para cada tipo de expresión y luego metemos todas en primary´
//Revisar detalles
private Expression ParseExpression(){
   
   
   
    return ParseLogic();
    
}
private Expression ParseLogic(){
    Expression expr=ParseEquality();

    while (Stream.Match(new List<TokenType>{TokenType.AND, TokenType.OR}))
    {
        Token thisoperator=Stream.Previous();
        Expression right=ParseEquality();
        if (thisoperator.Type==TokenType.AND)
        {
             expr=new ANDOperation(thisoperator.Location,expr,right);
        }
        else{
             expr=new OrOperation(thisoperator.Location,expr,right);
        }
        
    }
    return expr;
}

private Expression ParseEquality(){
    Expression expr=ParseComparison();

    while (Stream.Match(new List<TokenType>{TokenType.NOT_EQUAL, TokenType.EQUAL}))
    {
        Token thisoperator=Stream.Previous();
        Expression right=ParseComparison();

        if(thisoperator.Type==TokenType.NOT_EQUAL)
        {
            expr=new NotEqualToOperation(thisoperator.Location,expr,right);
        }
        else{
              expr=new EqualToOperation(thisoperator.Location,expr,right);
        }
       
    }
    return expr;
}

private Expression ParseComparison(){
    Expression expr=ParseTerm();

    while (Stream.Match(new List<TokenType>{TokenType.GREATER,TokenType.GREATER_EQUAL,TokenType.LESS,TokenType.LESS_EQUAL}))
    {
        Token thisoperator=Stream.Previous();
        Expression right=ParseTerm();

        if (thisoperator.Type==TokenType.GREATER)
        {
            expr=new GreatherThanOperation(thisoperator.Location,expr,right);
        }
        else if(thisoperator.Type==TokenType.GREATER_EQUAL)
        {
            expr=new GreatherThanOrEqualToOperation(thisoperator.Location,expr,right);
        }
        else if(thisoperator.Type==TokenType.LESS)
        {
            expr=new LessThanOperation(thisoperator.Location,expr,right);
        }
        else{
            expr=new LessThanOrEqualToOperation(thisoperator.Location,expr,right);
        }
        
    }
    return expr;
}

private Expression ParseTerm(){
    Expression expr=ParseFactor();
   
    while (Stream.Match(new List<TokenType>{TokenType.PLUS,TokenType.MINUS}))
    {   
        Token thisoperator=Stream.Previous();
        Expression right=ParseFactor();
        if (thisoperator.Type==TokenType.PLUS)
        {
            expr=new AdditionOperation(thisoperator.Location,expr,right);
        }
        else{
            expr=new SubstractionOperation(thisoperator.Location,expr,right);
        }
        
    }
    return expr;
}

private Expression ParseFactor(){
    Expression expr=ParseExponential();

    while (Stream.Match(new List<TokenType>{TokenType.MULTIPLY,TokenType.DIVIDE,TokenType.MODULO}))
    {
        Token thisoperator=Stream.Previous();
        Expression right=ParseExponential();
        if (thisoperator.Type==TokenType.MULTIPLY)
        {
            expr=new MultiplicationOperation(thisoperator.Location,expr,right);
        }
        else if(thisoperator.Type==TokenType.DIVIDE)
        {
            expr=new DivisionOperation(thisoperator.Location,expr,right);
        }
        else{
            expr=new ModuloOperation(thisoperator.Location,expr,right);
        }
        
        
    }
    return expr;
}

private Expression ParseExponential(){
    Expression expr=ParseUnary();

    while (Stream.Match(new List<TokenType>{TokenType.EXPONENTIAL}))
    {
        Token thisoperator=Stream.Previous();
        Expression right=ParseUnary();

       expr=new ExponentiationOperation(thisoperator.Location,expr,right);
    }
    return expr;
}

private Expression ParseUnary() {
    if (Stream.Match(new List<TokenType>{TokenType.NOT, TokenType.MINUS})) 
    {   Expression expr=new NotOperation(new CodeLocation(),null);;
        Token thisoperator = Stream.Previous();
        Expression right = ParseUnary();
        if (thisoperator.Type==TokenType.NOT)
        {
             expr=new NotOperation(thisoperator.Location,right);
        }
        else{
              expr=new NegationOperation(thisoperator.Location,right);
        }
        return expr;
    }
    return ParsePrimary();
}

    //Parsear funciones que retornan valor
    private Expression ParsePrimary()
    {
        if (Stream.Match(new List<TokenType> { TokenType.FALSE }))
        {
            return new Bool(Stream.Previous().Location, false);
        }
        if (Stream.Match(new List<TokenType> { TokenType.TRUE }))
        {
            return new Bool(Stream.Previous().Location, true);
        }
        if (Stream.Match(new List<TokenType> { TokenType.NUMBER }))
        {
            return new Number(Stream.Previous().Location, (int)Stream.Previous().Literal);
        }
        if (Stream.Match(new List<TokenType> { TokenType.STRING }))
        {
            return new StringLiteral(Stream.Previous().Location, (string)Stream.Previous().Literal);
        }
        if (Stream.Match(new List<TokenType> { TokenType.LEFT_PAREN }))
        {
            Expression expr = ParseExpression();
            Stream.Consume(TokenType.RIGHT_PAREN, ")");
            return new ParenthesizedExpression(Stream.Previous().Location, expr);
        }
        if (Stream.Match(new List<TokenType> { TokenType.ISBRUSHCOLOR, TokenType.ISBRUSHSIZE, TokenType.ISCANVASCOLOR, TokenType.ISCOLOR, TokenType.GETACTUALX, TokenType.GETACTUALY, TokenType.GETCANVASSIZE, TokenType.GETCOLORCOUNT }))

        {
            return ParseFunction();
        }
        if (Stream.Match(new List<TokenType> { TokenType.IDENTIFIER }))
        {
            return new Variable(Stream.Previous().Location, (string)Stream.Previous().Value);
        }

        throw SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "expression", Stream.Peek().Location);
}



#endregion

private void ParseIArgument(IArgument<Expression> argument){
     Stream.Consume(TokenType.LEFT_PAREN, "(");
if (!Stream.Check(TokenType.RIGHT_PAREN))
    {
         do{
            

            Expression? expr=ParseExpression();
           
            argument.Args.Add(expr);
         //  System.Console.WriteLine(expr.Type);
        
        
    }while(Stream.Match(new List<TokenType>{TokenType.COMMA}));
    
    }
   
   
    Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");
}
#region Command expression


private Command ParseCommand(){

    Token headCommand=Stream.Previous();
    
    Command? command= null;
    if (headCommand.Type==TokenType.COLOR)
    {
        command=new ColorCommand(headCommand.Location,TokenType.COLOR, new List<Expression>());
    }
    if (headCommand.Type==TokenType.DRAWCIRCLE)
    {
        command=new DrawCircleCommand(headCommand.Location,TokenType.DRAWCIRCLE, new List<Expression>());
    }
    if (headCommand.Type==TokenType.DRAWLINE)
    {
        command=new DrawLineCommand(headCommand.Location,TokenType.DRAWLINE, new List<Expression>());
    }
    if (headCommand.Type==TokenType.DRAWRECTANGLE)
    {
        command=new DrawRectangleCommand(headCommand.Location,TokenType.DRAWRECTANGLE, new List<Expression>());
    }
    if (headCommand.Type==TokenType.FILL)
    {
        command=new FillCommand(headCommand.Location,TokenType.FILL, new List<Expression>());
    }
    if (headCommand.Type==TokenType.SIZE)
    {
        command=new SizeCommand(headCommand.Location,TokenType.SIZE, new List<Expression>());
    }
    if (headCommand.Type==TokenType.SPAWN)
    {
        command=new SpawnCommand(headCommand.Location,TokenType.SPAWN, new List<Expression>());
    }
  

    ParseIArgument(command);
   
    return command; 
}
//Generar interface tiene argumentos para parsear en un solo metodo funciones y comandos
private Function ParseFunction(){

    Token headfunction=Stream.Previous();
    
    Function? function= null;
    if (headfunction.Type==TokenType.GETACTUALX)
    {
        function=new GetActualXFunction(headfunction.Location,TokenType.GETACTUALX, new List<Expression>());
    }
    if (headfunction.Type==TokenType.GETACTUALY)
    {
        function=new GetActualYFunction(headfunction.Location,TokenType.GETACTUALY, new List<Expression>());
    }
    if (headfunction.Type==TokenType.GETCANVASSIZE)
    {
        function=new GetCanvasSizeFunction(headfunction.Location,TokenType.GETCANVASSIZE, new List<Expression>());
    }
    if (headfunction.Type==TokenType.GETCOLORCOUNT)
    {
        function=new GetColorCountFunction(headfunction.Location,TokenType.GETCOLORCOUNT, new List<Expression>());
    }
    if (headfunction.Type==TokenType.ISBRUSHCOLOR)
    {
        function=new IsBrushColorFunction(headfunction.Location,TokenType.ISBRUSHCOLOR, new List<Expression>());
    }
    if (headfunction.Type==TokenType.ISBRUSHSIZE)
    {
        function=new IsBrushSizeFunction(headfunction.Location,TokenType.ISBRUSHSIZE, new List<Expression>());
    }
    if (headfunction.Type==TokenType.ISCANVASCOLOR)
    {
        function=new IsCanvasColor(headfunction.Location,TokenType.ISCANVASCOLOR, new List<Expression>());
    }
    if (headfunction.Type==TokenType.ISCOLOR)
    {
        function=new IsColorFunction(headfunction.Location,TokenType.ISCOLOR, new List<Expression>());
    }
   

    ParseIArgument(function);
    return function; 
}

private AssigmentExpression ParseAssignation(){
    Stream.MoveBack(1);
    Variable var= new Variable(Stream.Previous().Location, Stream.Previous().Value);
    Stream.Advance();
    Token assigment=Stream.Previous();
    
   
    return new AssigmentExpression(assigment.Location, var, ParseExpression());

    
}

private Command ParseGoTo(){
     Token headCommand=Stream.Previous();
    
    GoToCommand? command= new GoToCommand(headCommand.Location,TokenType.GOTO,new List<Expression>(),null);
    
      Stream.Consume(TokenType.LEFT_BRACKET, "[");
  

    if (Stream.Match(new List<TokenType>{TokenType.IDENTIFIER}))
    {  
        command.Label=Stream.Previous().Value;
    }
    else{
            throw SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Label", Stream.Peek().Location);
    }
     
    Stream.Consume(TokenType.RIGHT_BRACKET, "]");

    Stream.Consume(TokenType.LEFT_PAREN, "(");
    
    command.Args.Add(ParseExpression());
    
    Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");
 
    return command; 
}
#endregion
/*
    private Expression Expression(){
        return equality();
    }
*/
#region Parse program
public ElementalProgram Parse(){
    ElementalProgram program=new ElementalProgram(new CodeLocation(), Errors);
    while (!Stream.IsAtEnd())
    {
            if (Stream.Match(new List<TokenType> { TokenType.SPAWN }))
            {
                try
                {
                    program.Statements.Add(ParseCommand());
                  
                }
                catch (PixelWallEException error)
                {
                    program.Errors.Add(error);
                    Stream.Synchronize();
                }
                break;
            }
            else if (!Stream.Match(new List<TokenType> { TokenType.EOL, TokenType.COMENNT }))
            {
                program.Errors.Add(SyntaxException.SpawnMisplaced( Stream.Peek().Location));
                break;
            }
    }

        while (!Stream.IsAtEnd())
        {
            bool twoCommandLineError = false;
            if (Stream.Match(new List<TokenType> { TokenType.SPAWN }))
            {
                program.Errors.Add(SyntaxException.DuplicateSpawn(Stream.Peek().Location));
                Stream.Synchronize();
                continue;
            }
            else if (Stream.Match(new List<TokenType> { TokenType.COLOR, TokenType.DRAWCIRCLE, TokenType.DRAWLINE, TokenType.DRAWRECTANGLE, TokenType.FILL, TokenType.SIZE }))
            {
                try
                {
                    program.Statements.Add(ParseCommand());
                    twoCommandLineError = true;
                }
                catch (PixelWallEException error)
                {
                    program.Errors.Add(error);
                    Stream.Synchronize();
                    continue;
                }
            }
            else if (Stream.Match(new List<TokenType> { TokenType.GOTO }))
            {
                try
                {

                    program.Statements.Add(ParseGoTo());
                    twoCommandLineError = true;

                }
                catch (PixelWallEException error)
                {
                    program.Errors.Add(error);
                    Stream.Synchronize();
                    continue;

                }
            }
            else if (Stream.Match(new List<TokenType> { TokenType.IDENTIFIER }))
            {
                if (Stream.Match(new List<TokenType> { TokenType.ASSIGNMENT }))
                {
                    try
                    {
                        program.Statements.Add(ParseAssignation());
                        twoCommandLineError = true;
                    }
                    catch (PixelWallEException error)
                    {
                        program.Errors.Add(error);
                        Stream.Synchronize();
                        continue;
                    }
                }
                else
                {
                    program.Labels.Add(new Label(Stream.Previous().Value, program.Statements.Count()));
                    twoCommandLineError = true;
                }
            }
            if (!(Stream.Match(new List<TokenType> { TokenType.EOL }) || Stream.Match(new List<TokenType> { TokenType.EOF })))
            {
                if (twoCommandLineError)
                {
                    program.Errors.Add(SyntaxException.ExpectedNewLineAfterCommand(Stream.Peek().Value.ToString(), Stream.Peek().Location));
                    Stream.Synchronize();
                }
                else
                {
                    program.Errors.Add(SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Command or Label", Stream.Peek().Location ));
                    Stream.Synchronize();
                }

            }
            else if (Stream.Previous().Type == TokenType.EOF)
            {
                return program;
            }
            //Posible codigo sin uso
            
    
            //Verificar como compruebas quenno existan dos comandos en la misma linea si hay error en el primero sincronizas por lo cual no puedes comprobar el error y si no no lo haces


        }
           return program;
    
}


#endregion

    
}

/*public class PixelWallEException:System.Exception
{
    public Token? Token{get;}

    public PixelWallEException( Token token, string message): base($"[Line {token.Location.Line}:{token.Location.Column}] Parse Error: {message} (Found token: {token.Type}'{token.Value}')")
    {
        Token=token;
    }
    public PixelWallEException( CodeLocation location, string message): base($"[Line {location.Line}:{location.Column}] Parse Error: {message} )")
    {
       Token=default;
    }
    public PixelWallEException(string message):base($"Parse error: {message}"){
        Token=default;
    }
}*/