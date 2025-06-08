using PixelWallE.Language.Parsing.Expressions.Literals;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;




namespace PixelWallE.Language.Parsing;

/// <summary>
/// Represents a parser for the PixelWallE language.
/// This class is responsible for converting a stream of tokens into an Abstract Syntax Tree (AST).
/// </summary>
public class Parser
{
    /// <summary>
    /// Gets the token stream from which the parser reads tokens.
    /// </summary>
    public TokenStream Stream { get; private set; }
    /// <summary>
    /// Gets or sets a list to collect any parsing errors encountered.
    /// </summary>
    private List<PixelWallEException> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Parser"/> class.
    /// </summary>
    /// <param name="stream">The token stream to be parsed.</param>
    /// <param name="errors">A list to which parsing errors will be added.</param>
    /// 
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

    private Expression ParseExpression()
    {



        return ParseLogic();

    }
    private Expression ParseLogic()
    {
        Expression expr = ParseEquality();

        while (Stream.Match(new List<TokenType> { TokenType.AND, TokenType.OR }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseEquality();
            if (thisoperator.Type == TokenType.AND)
            {
                expr = new ANDOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new OrOperation(thisoperator.Location, expr, right);
            }

        }
        return expr;
    }

    private Expression ParseEquality()
    {
        Expression expr = ParseComparison();

        while (Stream.Match(new List<TokenType> { TokenType.NOT_EQUAL, TokenType.EQUAL }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseComparison();

            if (thisoperator.Type == TokenType.NOT_EQUAL)
            {
                expr = new NotEqualToOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new EqualToOperation(thisoperator.Location, expr, right);
            }

        }
        return expr;
    }

    private Expression ParseComparison()
    {
        Expression expr = ParseTerm();

        while (Stream.Match(new List<TokenType> { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseTerm();

            if (thisoperator.Type == TokenType.GREATER)
            {
                expr = new GreatherThanOperation(thisoperator.Location, expr, right);
            }
            else if (thisoperator.Type == TokenType.GREATER_EQUAL)
            {
                expr = new GreatherThanOrEqualToOperation(thisoperator.Location, expr, right);
            }
            else if (thisoperator.Type == TokenType.LESS)
            {
                expr = new LessThanOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new LessThanOrEqualToOperation(thisoperator.Location, expr, right);
            }

        }
        return expr;
    }

    private Expression ParseTerm()
    {
        Expression expr = ParseFactor();

        while (Stream.Match(new List<TokenType> { TokenType.PLUS, TokenType.MINUS }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseFactor();
            if (thisoperator.Type == TokenType.PLUS)
            {
                expr = new AdditionOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new SubstractionOperation(thisoperator.Location, expr, right);
            }

        }
        return expr;
    }

    private Expression ParseFactor()
    {
        Expression expr = ParseExponential();

        while (Stream.Match(new List<TokenType> { TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.MODULO }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseExponential();
            if (thisoperator.Type == TokenType.MULTIPLY)
            {
                expr = new MultiplicationOperation(thisoperator.Location, expr, right);
            }
            else if (thisoperator.Type == TokenType.DIVIDE)
            {
                expr = new DivisionOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new ModuloOperation(thisoperator.Location, expr, right);
            }


        }
        return expr;
    }

    private Expression ParseExponential()
    {
        Expression expr = ParseUnary();

        while (Stream.Match(new List<TokenType> { TokenType.EXPONENTIAL }))
        {
            Token thisoperator = Stream.Previous();
            Expression right = ParseUnary();

            expr = new ExponentiationOperation(thisoperator.Location, expr, right);
        }
        return expr;
    }

    private Expression ParseUnary()
    {
        if (Stream.Match(new List<TokenType> { TokenType.NOT, TokenType.MINUS }))
        {
            Expression expr = new NotOperation(new CodeLocation(), null); ;
            Token thisoperator = Stream.Previous();
            Expression right = ParseUnary();
            if (thisoperator.Type == TokenType.NOT)
            {
                expr = new NotOperation(thisoperator.Location, right);
            }
            else
            {
                expr = new NegationOperation(thisoperator.Location, right);
            }
            return expr;
        }
        return ParsePrimary();
    }


    private List ParseList()
    {
        Token headToken = Stream.Previous();

        Stream.Consume(TokenType.LESS, "<");
        TokenType argsType = Stream.Peek().Type;
        ExpressionType listType;
        if (argsType == TokenType.STRINGTYPE)
            listType = ExpressionType.ListString;
        else if (argsType == TokenType.INTTYPE)
            listType = ExpressionType.ListNumber;
        else if (argsType == TokenType.BOOLTYPE)
            listType = ExpressionType.ListBool;
        else
            throw SyntaxException.UnexpectedToken(argsType.ToString(), "string || int || bool", headToken.Location);
        Stream.Advance();
        Stream.Consume(TokenType.GREATER, ">");

        List list = new List(headToken.Location, null, listType);

      

        Stream.Consume(TokenType.LEFT_BRACKET, "[");
        ParseArgument(list, TokenType.RIGHT_BRACKET);
        Stream.Consume(TokenType.RIGHT_BRACKET, "]' or ',");
        return list;
    }

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
        if (Stream.Match(new List<TokenType> { TokenType.ISBRUSHCOLOR, TokenType.ISBRUSHSIZE, TokenType.ISCANVASCOLOR, TokenType.GETACTUALX, TokenType.GETACTUALY, TokenType.GETCANVASSIZE, TokenType.GETCOLORCOUNT }))

        {
            return (Function)ParseCommandorFunction();
        }
        if (Stream.Match(new List<TokenType> { TokenType.LIST }))
        {
            return ParseList();
        }
        if (Stream.Match(new List<TokenType> { TokenType.IDENTIFIER }))
        { Token identifier = Stream.Previous();
            if (Stream.Match(new List<TokenType> { TokenType.LEFT_BRACKET }))
            {
                Expression index = ParseExpression();
                Stream.Consume(TokenType.RIGHT_BRACKET, "]");
                
                return new ListElement(identifier.Location, identifier.Value, index);
            }
            if (Stream.Match(new List<TokenType> { TokenType.DOT }))
            {
                  if (Stream.Match(new List<TokenType> { TokenType.COUNT }))
            {

                return (Function)ParseListCommand();
            }
            }
          
            return new Variable(identifier.Location, identifier.Value);
        }

        throw SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "expression", Stream.Peek().Location);
    }



    #endregion

    private void ParseIArgument(IArgument<Expression> argument)
    {
        Stream.Consume(TokenType.LEFT_PAREN, "(");
        ParseArgument(argument, TokenType.RIGHT_PAREN);


        Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");
      
    }
    private void ParseArgument(IArgument<Expression> argument, TokenType end)
    {
       
        if (!Stream.Check(end))
        {   
            do
            {


                Expression? expr = ParseExpression();

                argument.Args.Add(expr);



            } while (Stream.Match(new List<TokenType> { TokenType.COMMA }));

        }
    }
    #region Command expression


    private IArgument<Expression> ParseCommandorFunction()
    {

        Token headCommand = Stream.Previous();

        ASTNode? command = null;

        command = CommandFunctionProvider(headCommand);


        IArgument<Expression> argument = (IArgument<Expression>)command;

        ParseIArgument(argument);

        return argument;
    }

    private IArgument<Expression> ParseListCommand()
    {
        Stream.MoveBack(2);
        Token headCommand = Stream.Previous();
        Stream.MoveNext(2);
        
        ASTNode? command = CommandFunctionProvider(Stream.Previous());
        IArgument<Expression> argument = (IArgument<Expression>)command;

        ParseIArgument(argument);
        foreach (var item in argument.Args)
        {
            Godot.GD.Print(item);
        }
        IListReference listCommand = (IListReference)argument;
        Godot.GD.Print("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
        Godot.GD.Print(headCommand.Value.ToString());
        listCommand.ListReference=headCommand.Value.ToString();

        return argument;
    }

    private AssigmentExpression ParseAssignation()
    {
        Stream.MoveBack(1);
        Variable var = new Variable(Stream.Previous().Location, Stream.Previous().Value);
        Stream.Advance();
        Token assigment = Stream.Previous();


        return new AssigmentExpression(assigment.Location, var, ParseExpression());


    }

    private Command ParseGoTo()
    {
        Token headCommand = Stream.Previous();

        GoToCommand? command = new GoToCommand(headCommand.Location, TokenType.GOTO, new List<Expression>(), null);

        Stream.Consume(TokenType.LEFT_BRACKET, "[");


        if (Stream.Match(new List<TokenType> { TokenType.IDENTIFIER }))
        {
            command.Label = Stream.Previous().Value;
        }
        else
        {
            throw SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Label", Stream.Peek().Location);
        }

        Stream.Consume(TokenType.RIGHT_BRACKET, "]");

        Stream.Consume(TokenType.LEFT_PAREN, "(");

        command.Args.Add(ParseExpression());

        Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");

        return command;
    }
    #endregion

    #region Parse program
    public ElementalProgram Parse()
    {
        ElementalProgram program = new ElementalProgram(new CodeLocation(), Errors);
        HashSet<string> referenceLabel = new HashSet<string>();

        while (!Stream.IsAtEnd())
        {
            if (Stream.Match(new List<TokenType> { TokenType.SPAWN }))
            {
                try
                {
                    program.Statements.Add((Command)ParseCommandorFunction());
                }
                catch (PixelWallEException error)
                {
                    program.Errors.Add(error);
                    Stream.Synchronize();
                }


                break;
            }
            else if (!Stream.Match(new List<TokenType> { TokenType.EOL }))
            {

                program.Errors.Add(SyntaxException.SpawnMisplaced(Stream.Peek().Location));
                Stream.Synchronize();
                break;
            }
        }

        while (!Stream.IsAtEnd())
        {
            bool twoCommandLineError = false;
            if (Stream.Peek().Location.Line == 1 && !Stream.Match(new List<TokenType> { TokenType.EOL }))
            {
                program.Errors.Add(SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Command or Label", Stream.Peek().Location));
            }
            if (Stream.Match(new List<TokenType> { TokenType.SPAWN }))
            {
                program.Errors.Add(SyntaxException.DuplicateSpawn(Stream.Peek().Location));
                Stream.Synchronize();
                continue;
            }
            else if (Stream.Match(new List<TokenType> { TokenType.COLOR, TokenType.DRAWCIRCLE, TokenType.DRAWLINE, TokenType.DRAWRECTANGLE, TokenType.FILL, TokenType.SIZE, TokenType.PRINT, TokenType.RESPAWN , TokenType.RUN}))
            {
                try
                {
                    program.Statements.Add((Command)ParseCommandorFunction());
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
                else if (Stream.Match(new List<TokenType> { TokenType.DOT }))
                {
                    Godot.GD.Print("Vamos a tratar de parsear un ocmando de lsita");
                    if (Stream.Match(new List<TokenType> { TokenType.ADD, TokenType.CLEAR, TokenType.REMOVEAT }))
                    {

                        try
                        {   
                            program.Statements.Add((ListCommand)ParseListCommand());
                            twoCommandLineError = true;
                        }
                        catch (PixelWallEException error)
                        {
                            program.Errors.Add(error);
                            Stream.Synchronize();
                            continue;
                        }
                    }
                }
                else
                {
                    if (referenceLabel.Contains(Stream.Previous().Value))
                    {
                        program.Errors.Add(SyntaxException.DuplicateLabel(Stream.Previous().Location, Stream.Previous().Value));
                        Stream.Synchronize();
                        continue;
                    }
                    else
                    {
                        program.Labels.Add(new Label(Stream.Previous().Value, program.Statements.Count()));
                        referenceLabel.Add(Stream.Previous().Value);
                        twoCommandLineError = true;
                    }

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
                    program.Errors.Add(SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Command or Label", Stream.Peek().Location));
                    Stream.Synchronize();
                }

            }


        }
        return program;

    }


    #endregion

    private ASTNode? CommandFunctionProvider(Token headCommand)
    {
        if (headCommand.Type == TokenType.COLOR)
        {
            return new ColorCommand(headCommand.Location, TokenType.COLOR, new List<Expression>());
        }
        if (headCommand.Type == TokenType.DRAWCIRCLE)
        {
            return new DrawCircleCommand(headCommand.Location, TokenType.DRAWCIRCLE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.DRAWLINE)
        {
            return new DrawLineCommand(headCommand.Location, TokenType.DRAWLINE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.DRAWRECTANGLE)
        {
            return new DrawRectangleCommand(headCommand.Location, TokenType.DRAWRECTANGLE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.FILL)
        {
            return new FillCommand(headCommand.Location, TokenType.FILL, new List<Expression>());
        }
        if (headCommand.Type == TokenType.SIZE)
        {
            return new SizeCommand(headCommand.Location, TokenType.SIZE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.SPAWN)
        {
            return new SpawnCommand(headCommand.Location, TokenType.SPAWN, new List<Expression>());
        }
        if (headCommand.Type == TokenType.RESPAWN)
        {
            return new ReSpawnCommand(headCommand.Location, TokenType.RESPAWN, new List<Expression>());
        }
        if (headCommand.Type == TokenType.PRINT)
        {
            return new PrintCommand(headCommand.Location, TokenType.PRINT, new List<Expression>());
        }

        if (headCommand.Type == TokenType.ADD)
        {
            return new AddCommand(headCommand.Location, TokenType.ADD, new List<Expression>());
        }

        if (headCommand.Type == TokenType.CLEAR)
        {
            return new ClearCommand(headCommand.Location, TokenType.CLEAR, new List<Expression>());
        }
        if (headCommand.Type == TokenType.REMOVEAT)
        {
            return new RemoveAtCommand(headCommand.Location, TokenType.REMOVEAT, new List<Expression>());
        }
        if (headCommand.Type == TokenType.COUNT)
        {
            return new CountCommand(headCommand.Location, TokenType.COUNT, new List<Expression>());
        }








        if (headCommand.Type == TokenType.GETACTUALX)
        {
            return new GetActualXFunction(headCommand.Location, TokenType.GETACTUALX, new List<Expression>());
        }
        if (headCommand.Type == TokenType.GETACTUALY)
        {
            return new GetActualYFunction(headCommand.Location, TokenType.GETACTUALY, new List<Expression>());
        }
        if (headCommand.Type == TokenType.GETCANVASSIZE)
        {
            return new GetCanvasSizeFunction(headCommand.Location, TokenType.GETCANVASSIZE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.GETCOLORCOUNT)
        {
            return new GetColorCountFunction(headCommand.Location, TokenType.GETCOLORCOUNT, new List<Expression>());
        }
        if (headCommand.Type == TokenType.ISBRUSHCOLOR)
        {
            return new IsBrushColorFunction(headCommand.Location, TokenType.ISBRUSHCOLOR, new List<Expression>());
        }
        if (headCommand.Type == TokenType.ISBRUSHSIZE)
        {
            return new IsBrushSizeFunction(headCommand.Location, TokenType.ISBRUSHSIZE, new List<Expression>());
        }
        if (headCommand.Type == TokenType.ISCANVASCOLOR)
        {
            return new IsCanvasColor(headCommand.Location, TokenType.ISCANVASCOLOR, new List<Expression>());
        }
         if (headCommand.Type == TokenType.RUN)
        {
            return new RunCommand(headCommand.Location, TokenType.RUN, new List<Expression>());
        }
        return null;
    }

}

