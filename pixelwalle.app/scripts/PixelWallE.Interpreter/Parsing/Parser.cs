using PixelWallE.Language.Parsing.Expressions.Literals;
using PixelWallE.Language.Parsing.Expressions;
using PixelWallE.Language.Tokens;
using PixelWallE.Language.Expressions;
using PixelWallE.Language.Commands;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PixelWallE.Language;

namespace PixelWallE.Language.Parsing;

/// <summary>
/// The Parser class is responsible for parsing a stream of tokens into an abstract syntax tree (AST).
/// It interprets the structure of the language and converts it into meaningful expressions and commands.
/// </summary>
public class Parser
{
    /// <summary>
    /// Gets or sets the token stream to be parsed.
    /// </summary>
    public TokenStream Stream { get; private set; }

    /// <summary>
    /// Gets or sets the list of errors encountered during parsing.
    /// </summary>
    private List<PixelWallEException> Errors { get; set; }

    /// <summary>
    /// Initializes a new instance of the Parser class.
    /// </summary>
    /// <param name="stream">The token stream to parse.</param>
    /// <param name="errors">A list to store parsing errors.</param>
    public Parser(TokenStream stream, List<PixelWallEException> errors)
    {
        Stream = stream;
        Errors = errors;
    }

    #region Expression parsing

    /// <summary>
    /// Parses an expression from the token stream.
    /// </summary>
    /// <returns>An Expression object representing the parsed expression.</returns>
    private Expression ParseExpression()
    {
        // The top-level expression parsing starts with logical expressions.
        return ParseLogic();
    }

    /// <summary>
    /// Parses logical expressions (AND, OR).
    /// </summary>
    /// <returns>An Expression representing the parsed logical expression.</returns>
    private Expression ParseLogic()
    {
        // Start by parsing an equality expression.
        Expression expr = ParseEquality();

        // Continue parsing as long as AND or OR tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.AND, TokenType.OR }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side equality expression.
            Expression right = ParseEquality();

            // Create the appropriate operation based on the operator type.
            if (thisoperator.Type == TokenType.AND)
            {
                expr = new ANDOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new OrOperation(thisoperator.Location, expr, right);
            }
        }
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses equality expressions (==, !=).
    /// </summary>
    /// <returns>An Expression representing the parsed equality expression.</returns>
    private Expression ParseEquality()
    {
        // Start by parsing a comparison expression.
        Expression expr = ParseComparison();

        // Continue parsing as long as NOT_EQUAL or EQUAL tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.NOT_EQUAL, TokenType.EQUAL }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side comparison expression.
            Expression right = ParseComparison();

            // Create the appropriate operation based on the operator type.
            if (thisoperator.Type == TokenType.NOT_EQUAL)
            {
                expr = new NotEqualToOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new EqualToOperation(thisoperator.Location, expr, right);
            }
        }
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses comparison expressions (&lt;, &lt;=, &gt;, &gt;=).
    /// </summary>
    /// <returns>An Expression representing the parsed comparison expression.</returns>
    private Expression ParseComparison()
    {
        // Start by parsing a term expression.
        Expression expr = ParseTerm();

        // Continue parsing as long as GREATER, GREATER_EQUAL, LESS, or LESS_EQUAL tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side term expression.
            Expression right = ParseTerm();

            // Create the appropriate operation based on the operator type.
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
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses term expressions (+, -).
    /// </summary>
    /// <returns>An Expression representing the parsed term expression.</returns>
    private Expression ParseTerm()
    {
        // Start by parsing a factor expression.
        Expression expr = ParseFactor();

        // Continue parsing as long as PLUS or MINUS tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.PLUS, TokenType.MINUS }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side factor expression.
            Expression right = ParseFactor();

            // Create the appropriate operation based on the operator type.
            if (thisoperator.Type == TokenType.PLUS)
            {
                expr = new AdditionOperation(thisoperator.Location, expr, right);
            }
            else
            {
                expr = new SubstractionOperation(thisoperator.Location, expr, right);
            }
        }
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses factor expressions (*, /, %).
    /// </summary>
    /// <returns>An Expression representing the parsed factor expression.</returns>
    private Expression ParseFactor()
    {
        // Start by parsing an exponential expression.
        Expression expr = ParseExponential();

        // Continue parsing as long as MULTIPLY, DIVIDE, or MODULO tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.MULTIPLY, TokenType.DIVIDE, TokenType.MODULO }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side exponential expression.
            Expression right = ParseExponential();

            // Create the appropriate operation based on the operator type.
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
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses exponential expressions (^).
    /// </summary>
    /// <returns>An Expression representing the parsed exponential expression.</returns>
    private Expression ParseExponential()
    {
        // Start by parsing a unary expression.
        Expression expr = ParseUnary();

        // Continue parsing as long as EXPONENTIAL tokens are encountered.
        while (Stream.Match(new List<TokenType> { TokenType.EXPONENTIAL }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the right-hand side unary expression.
            Expression right = ParseUnary();

            // Create an exponentiation operation.
            expr = new ExponentiationOperation(thisoperator.Location, expr, right);
        }
        // Return the resulting expression.
        return expr;
    }

    /// <summary>
    /// Parses unary expressions (!, -).
    /// </summary>
    /// <returns>An Expression representing the parsed unary expression.</returns>
    private Expression ParseUnary()
    {
        // Check if the current token is a NOT or MINUS.
        if (Stream.Match(new List<TokenType> { TokenType.NOT, TokenType.MINUS }))
        {
            // Store the operator token.
            Token thisoperator = Stream.Previous();
            // Parse the unary expression.
            Expression right = ParseUnary();

            // Create the appropriate operation based on the operator type.
            if (thisoperator.Type == TokenType.NOT)
            {
                return new NotOperation(thisoperator.Location, right);
            }
            else
            {
                return new NegationOperation(thisoperator.Location, right);
            }
        }
        // If no unary operator is found, parse a primary expression.
        return ParsePrimary();
    }

    /// <summary>
    /// Parses a list expression.
    /// </summary>
    /// <returns>An Expression representing the parsed list expression.</returns>
    private List ParseList()
    {
        // Get the head token (LIST keyword).
        Token headToken = Stream.Previous();

        // Consume the opening angle bracket.
        Stream.Consume(TokenType.LESS, "<");
        // Determine the type of the list based on the token following the opening angle bracket.
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
        // Advance the stream and consume the closing angle bracket.
        Stream.Advance();
        Stream.Consume(TokenType.GREATER, ">");

        // Create a new list expression.
        List list = new List(headToken.Location, null, listType);

        // Consume the opening square bracket and parse the arguments.
        Stream.Consume(TokenType.LEFT_BRACKET, "[");
        ParseArgument(list, TokenType.RIGHT_BRACKET);
        // Consume the closing square bracket.
        Stream.Consume(TokenType.RIGHT_BRACKET, "]' or ',");
        // Return the list expression.
        return list;
    }

    /// <summary>
    /// Parses primary expressions (literals, identifiers, parenthesized expressions, etc.).
    /// </summary>
    /// <returns>An Expression representing the parsed primary expression.</returns>
    private Expression ParsePrimary()
    {
        // Match various token types and return corresponding expressions.
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
        {
            Token identifier = Stream.Previous();
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

    /// <summary>
    /// Parses an argument enclosed in parentheses.
    /// </summary>
    /// <param name="argument">The argument to parse.</param>
    private void ParseIArgument(IArgument<Expression> argument)
    {
        // Consume the opening parenthesis.
        Stream.Consume(TokenType.LEFT_PAREN, "(");
        // Parse the argument.
        ParseArgument(argument, TokenType.RIGHT_PAREN);

        // Consume the closing parenthesis.
        Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");
    }

    /// <summary>
    /// Parses a list of arguments.
    /// </summary>
    /// <param name="argument">The argument list to populate.</param>
    /// <param name="end">The token type that marks the end of the argument list.</param>
    private void ParseArgument(IArgument<Expression> argument, TokenType end)
    {
        // Continue parsing arguments as long as the end token is not encountered.
        if (!Stream.Check(end))
        {
            do
            {
                // Parse an expression.
                Expression? expr = ParseExpression();

                // Add the expression to the argument list.
                argument.Args.Add(expr);
            } while (Stream.Match(new List<TokenType> { TokenType.COMMA })); // Continue if a comma is encountered.
        }
    }

    #region Command expression

    /// <summary>
    /// Parses a command or function call.
    /// </summary>
    /// <returns>An IArgument representing the parsed command or function call.</returns>
    private IArgument<Expression> ParseCommandorFunction()
    {
        // Get the head command token.
        Token headCommand = Stream.Previous();

        // Get the command from the provider.
        ASTNode? command = CommandFunctionProvider(headCommand);

        // Cast the command to an IArgument.
        IArgument<Expression> argument = (IArgument<Expression>)command;

        // Parse the arguments for the command.
        ParseIArgument(argument);

        // Return the argument.
        return argument;
    }

    /// <summary>
    /// Parses a list command.
    /// </summary>
    /// <returns>An IArgument representing the parsed list command.</returns>
    private IArgument<Expression> ParseListCommand()
    {
        // Move back two tokens to get the identifier.
        Stream.MoveBack(2);
        Token headCommand = Stream.Previous();
        // Move forward two tokens to continue parsing.
        Stream.MoveNext(2);

        // Get the command from the provider.
        ASTNode? command = CommandFunctionProvider(Stream.Previous());
        // Cast the command to an IArgument.
        IArgument<Expression> argument = (IArgument<Expression>)command;

        // Parse the arguments for the command.
        ParseIArgument(argument);
        // Print each argument (for debugging).
        foreach (var item in argument.Args)
        {
            Godot.GD.Print(item);
        }
        // Cast the argument to an IListReference.
        IListReference listCommand = (IListReference)argument;
        // Set the list reference.
        listCommand.ListReference = headCommand.Value.ToString();

        // Return the argument.
        return argument;
    }

    /// <summary>
    /// Parses an assignment expression.
    /// </summary>
    /// <returns>An AssigmentExpression representing the parsed assignment expression.</returns>
    private AssigmentExpression ParseAssignation()
    {
        // Move back one token to get the identifier.
        Stream.MoveBack(1);
        // Create a variable from the identifier.
        Variable var = new Variable(Stream.Previous().Location, Stream.Previous().Value);
        // Move forward to continue parsing.
        Stream.Advance();
        // Get the assignment token.
        Token assigment = Stream.Previous();

        // Return a new assignment expression.
        return new AssigmentExpression(assigment.Location, var, ParseExpression());
    }

     private AssigmentListElement ParseListAssignation()
    {
        // Move back one token to get the identifier.
        Stream.MoveBack(1);
        // Create a variable from the identifier.
        Variable var = new Variable(Stream.Previous().Location, Stream.Previous().Value);
        // Move forward to continue parsing.
        Stream.Consume(TokenType.LEFT_BRACKET, "[");
        Expression index = ParseExpression();
        Stream.Consume(TokenType.RIGHT_BRACKET, "]");
        // Get the assignment token.
        
        
        Stream.Consume(TokenType.ASSIGNMENT, "<-");
        Token assigment = Stream.Previous();
        // Return a new assignment expression.
        return new AssigmentListElement(assigment.Location, var, ParseExpression(), index);
    }

    /// <summary>
    /// Parses a GoTo command.
    /// </summary>
    /// <returns>A Command representing the parsed GoTo command.</returns>
    private Command ParseGoTo()
    {
        // Get the head command token.
        Token headCommand = Stream.Previous();

        // Create a new GoTo command.
        GoToCommand? command = new GoToCommand(headCommand.Location, TokenType.GOTO, new List<Expression>(), null);

        // Consume the opening square bracket.
        Stream.Consume(TokenType.LEFT_BRACKET, "[");

        // Check if the next token is an identifier.
        if (Stream.Match(new List<TokenType> { TokenType.IDENTIFIER }))
        {
            // Set the label to the identifier value.
            command.Label = Stream.Previous().Value;
        }
        else
        {
            // Throw an exception if the token is not an identifier.
            throw SyntaxException.UnexpectedToken(Stream.Peek().Value.ToString(), "Label", Stream.Peek().Location);
        }

        // Consume the closing square bracket.
        Stream.Consume(TokenType.RIGHT_BRACKET, "]");

        // Consume the opening parenthesis.
        Stream.Consume(TokenType.LEFT_PAREN, "(");

        // Add the parsed expression to the arguments.
        command.Args.Add(ParseExpression());

        // Consume the closing parenthesis.
        Stream.Consume(TokenType.RIGHT_PAREN, ")' or ',");

        // Return the command.
        return command;
    }

    #endregion

    #region Parse program

    /// <summary>
    /// Parses the entire program.
    /// </summary>
    /// <returns>An ElementalProgram representing the parsed program.</returns>
    public ElementalProgram Parse()
    {
        // Create a new elemental program.
        ElementalProgram program = new ElementalProgram(new CodeLocation(), Errors);
        // Create a hash set to store reference labels.
        HashSet<string> referenceLabel = new HashSet<string>();

        // Parse the spawn command.
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

        // Parse the rest of the program.
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
            else if (Stream.Match(new List<TokenType> { TokenType.COLOR, TokenType.DRAWCIRCLE, TokenType.DRAWLINE, TokenType.DRAWRECTANGLE, TokenType.FILL, TokenType.SIZE, TokenType.PRINT, TokenType.RESPAWN, TokenType.RUN }))
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
                else if (Stream.Match(new List<TokenType> { TokenType.LEFT_BRACKET }))
                {
                     try
                    {
                        program.Statements.Add(ParseListAssignation());
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
        // Return the program.
        return program;
    }

    #endregion

    /// <summary>
    /// Provides the appropriate command or function object based on the token type.
    /// </summary>
    /// <param name="headCommand">The token representing the command or function.</param>
    /// <returns>An ASTNode representing the command or function, or null if not found.</returns>
    private ASTNode? CommandFunctionProvider(Token headCommand)
    {
        // Check the token type and return the corresponding command or function.
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
        // If the token type is not recognized, return null.
        return null;
    }
}

