namespace PixelWallE.Language.Lexer;

using PixelWallE.Language.Tokens;


/// <summary>
/// Provides a singleton instance of the <see cref="Lexer"/> class, pre-configured with operators and keywords
/// specific to the PixelWallE language. The <c>Lexical</c> property initializes the lexer on first access,
/// registering all supported operators, keywords, and text delimiters.
/// </summary>
/// <remarks>
/// The lexer is configured with common arithmetic, logical, comparison, and assignment operators,
/// as well as language-specific keywords and text delimiters. This ensures consistent lexical analysis
/// throughout the application.
/// </remarks>
public class LexerProvider

{
    private static Lexer? __LexicalProcess;
    public static Lexer Lexical
    {
        get
        {
            if (__LexicalProcess == null)
            {
                __LexicalProcess = new Lexer();


                __LexicalProcess.RegisterOperator("+", TokenType.PLUS);
                __LexicalProcess.RegisterOperator("*", TokenType.MULTIPLY);
                __LexicalProcess.RegisterOperator("**", TokenType.EXPONENTIAL);
                __LexicalProcess.RegisterOperator("%", TokenType.MODULO);
                __LexicalProcess.RegisterOperator("-", TokenType.MINUS);
                __LexicalProcess.RegisterOperator("/", TokenType.DIVIDE);
                __LexicalProcess.RegisterOperator("<-", TokenType.ASSIGNMENT);
                __LexicalProcess.RegisterOperator("&&", TokenType.AND);
                __LexicalProcess.RegisterOperator("||", TokenType.OR);
                __LexicalProcess.RegisterOperator("==", TokenType.EQUAL);
                __LexicalProcess.RegisterOperator("!=", TokenType.NOT_EQUAL);
                __LexicalProcess.RegisterOperator("!", TokenType.NOT);

                __LexicalProcess.RegisterOperator(">", TokenType.GREATER);
                __LexicalProcess.RegisterOperator(">=", TokenType.GREATER_EQUAL);
                __LexicalProcess.RegisterOperator("<", TokenType.LESS);
                __LexicalProcess.RegisterOperator("<=", TokenType.LESS_EQUAL);

                __LexicalProcess.RegisterOperator(",", TokenType.COMMA);
                __LexicalProcess.RegisterOperator("(", TokenType.LEFT_PAREN);
                __LexicalProcess.RegisterOperator(")", TokenType.RIGHT_PAREN);
                __LexicalProcess.RegisterOperator("[", TokenType.LEFT_BRACKET);
                __LexicalProcess.RegisterOperator("]", TokenType.RIGHT_BRACKET);
                __LexicalProcess.RegisterKeyword(".", TokenType.DOT);
                __LexicalProcess.RegisterOperator("\n", TokenType.NEW_LINE);




                __LexicalProcess.RegisterKeyword("false", TokenType.FALSE);
                __LexicalProcess.RegisterKeyword("true", TokenType.TRUE);
                __LexicalProcess.RegisterKeyword("int", TokenType.INTTYPE);
                __LexicalProcess.RegisterKeyword("bool", TokenType.BOOLTYPE);
                __LexicalProcess.RegisterKeyword("string", TokenType.STRINGTYPE);
                __LexicalProcess.RegisterKeyword("List", TokenType.LIST);
                __LexicalProcess.RegisterKeyword("Clear", TokenType.CLEAR);
                __LexicalProcess.RegisterKeyword("RemoveAt", TokenType.REMOVEAT);
                __LexicalProcess.RegisterKeyword("Add", TokenType.ADD);
                __LexicalProcess.RegisterKeyword("Lenght", TokenType.LENGHT);
                __LexicalProcess.RegisterKeyword("Color", TokenType.COLOR);
                __LexicalProcess.RegisterKeyword("DrawCircle", TokenType.DRAWCIRCLE);
                __LexicalProcess.RegisterKeyword("DrawLine", TokenType.DRAWLINE);
                __LexicalProcess.RegisterKeyword("DrawRectangle", TokenType.DRAWRECTANGLE);
                __LexicalProcess.RegisterKeyword("GoTo", TokenType.GOTO);
                __LexicalProcess.RegisterKeyword("Size", TokenType.SIZE);
                __LexicalProcess.RegisterKeyword("Spawn", TokenType.SPAWN);
                __LexicalProcess.RegisterKeyword("ReSpawn", TokenType.RESPAWN);
                __LexicalProcess.RegisterKeyword("Fill", TokenType.FILL);
                __LexicalProcess.RegisterKeyword("Print", TokenType.PRINT);
                __LexicalProcess.RegisterKeyword("GetActualX", TokenType.GETACTUALX);
                __LexicalProcess.RegisterKeyword("GetActualY", TokenType.GETACTUALY);
                __LexicalProcess.RegisterKeyword("GetCanvasSize", TokenType.GETCANVASSIZE);
                __LexicalProcess.RegisterKeyword("GetColorCount", TokenType.GETCOLORCOUNT);
                __LexicalProcess.RegisterKeyword("IsBrushColor", TokenType.ISBRUSHCOLOR);
                __LexicalProcess.RegisterKeyword("IsBrushSize", TokenType.ISBRUSHSIZE);
                __LexicalProcess.RegisterKeyword("IsCanvasColor", TokenType.ISCANVASCOLOR);

                /*  */
                __LexicalProcess.RegisterText("\"", "\"");
            }
            __LexicalProcess.OrderedDictionary();
            return __LexicalProcess;
        }
    }
}