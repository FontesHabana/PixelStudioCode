namespace PixelWallE.Language.Lexer;
using PixelWallE.Language.Tokens;

//Define patrón singleton para el lexer// De esto hablo el profe en la clase del juego, te aseguras de tener un lexer único para cada iteración de el código
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
                __LexicalProcess.RegisterOperator("\n", TokenType.NEW_LINE);
                
                


                __LexicalProcess.RegisterKeyword("false", TokenType.FALSE);
                __LexicalProcess.RegisterKeyword("true", TokenType.TRUE);
                __LexicalProcess.RegisterKeyword("Color", TokenType.COLOR);
                __LexicalProcess.RegisterKeyword("DrawCircle", TokenType.DRAWCIRCLE);
                __LexicalProcess.RegisterKeyword("DrawLine", TokenType.DRAWLINE);                
                __LexicalProcess.RegisterKeyword("DrawRectangle", TokenType.DRAWRECTANGLE);
                __LexicalProcess.RegisterKeyword("GoTo", TokenType.GOTO);
                __LexicalProcess.RegisterKeyword("Size", TokenType.SIZE);
                __LexicalProcess.RegisterKeyword("Spawn", TokenType.SPAWN);
                __LexicalProcess.RegisterKeyword("Fill", TokenType.FILL);
                __LexicalProcess.RegisterKeyword("GetActualX", TokenType.GETACTUALX);
                __LexicalProcess.RegisterKeyword("GetActualY", TokenType.GETACTUALY);
                __LexicalProcess.RegisterKeyword("GetCanvasSize", TokenType.GETCANVASSIZE);                
                __LexicalProcess.RegisterKeyword("GetColorCount", TokenType.GETCOLORCOUNT);
                __LexicalProcess.RegisterKeyword("IsBrushColor", TokenType.ISBRUSHCOLOR);
                __LexicalProcess.RegisterKeyword("IsBrushSize", TokenType.ISBRUSHSIZE);
                __LexicalProcess.RegisterKeyword("IsCanvasColor", TokenType.ISCANVASCOLOR);
                __LexicalProcess.RegisterKeyword("IsColor", TokenType.ISCOLOR);

                /*  */
                __LexicalProcess.RegisterText("\"", "\"");
            }

            return __LexicalProcess;
        }
    }
}